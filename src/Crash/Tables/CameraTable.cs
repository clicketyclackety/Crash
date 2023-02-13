using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Rhino.Display;
using Rhino.Geometry;
using Crash.Events;
using Crash.Document;
using System.Collections;
using Microsoft.VisualStudio.Threading;
using System.Threading;

namespace Crash.Tables
{

    public sealed class FixedSizedQueue<T> : IReadOnlyCollection<T>
    {
        private readonly Queue<T> _queue;

        public readonly int Size;

        public FixedSizedQueue(int size)
        {
            Size = size;
            _queue = new Queue<T>();
        }

        public void Enqueue(T item)
        {
            if (_queue.Count >= Size)
            {
                _queue.Dequeue();
            }

            _queue.Enqueue(item);
        }

        public int Count => _queue.Count;

        public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator();
    }

    public sealed class CameraTable : IEnumerable<Camera>
    {
        private const int MINIMUM_DISPLACEMENT = 1000; // What unit?
        private const int CHANGE_DELAY = 200; // in milliseconds
        private const int FOLLOW_DELAY = 750; // in milliseconds
        private const int FPS = 30; // TODO: Make this a setting
        private const int MAX_CAMERAS_IN_QUEUE = 3;

        private CrashDoc crashDoc;

        // TODO: Don't use static
        private static DateTime lastChange = DateTime.Now;
        private static Point3d lastLocation = Point3d.Unset;
        private static Point3d lastTarget = Point3d.Unset;

        private Dictionary<string, FixedSizedQueue<Camera>> cameraLocations;

        private Curve cameraLocationRail;
        private Curve cameraTargetRail;

        private bool needsNewRails = false;

        public CameraTable(CrashDoc hostDoc)
        {
            cameraLocations = new Dictionary<string, FixedSizedQueue<Camera>>();
            this.crashDoc = hostDoc;
        }

        // Intentionally Static
        internal static void RhinoView_Modified(object sender, ViewEventArgs v)
        {
            if (null == CrashDoc.ActiveDoc?.LocalClient) return;
            if (CrashDoc.ActiveDoc.Users.Any(u => u.Camera == CameraState.Follow)) return;

            RhinoView view = v.View;
            Point3d cameraLocation = view.ActiveViewport.CameraLocation;
            Point3d cameraTarget = view.ActiveViewport.CameraTarget;
            DateTime currentChange = DateTime.UtcNow;

            if ((currentChange - lastChange).TotalMilliseconds < CHANGE_DELAY) return;

            // Limit the number of Specks we send
            if (cameraLocation.DistanceTo(lastLocation) < MINIMUM_DISPLACEMENT ||
                cameraTarget.DistanceTo(lastTarget) < MINIMUM_DISPLACEMENT)
            {
                lastChange = currentChange;

                Camera camera = new Camera(cameraLocation, cameraTarget);
                CameraSpeck cameraSpeck = CameraSpeck.CreateNew(camera, CrashDoc.ActiveDoc.Users.CurrentUser.Name);
                Speck serverSpeck = new Speck(cameraSpeck);

                Task.Run(() => CrashDoc.ActiveDoc.LocalClient.CameraChange(serverSpeck));
            }
        }

        internal void OnCameraChange(string userName, Speck cameraSpeck)
        {
            if (string.IsNullOrEmpty(userName)) return;
            User? user = crashDoc.Users.Get(userName);
            if (null == user) return;
            Camera? newCamera = Camera.FromJSON(cameraSpeck.Payload);
            if (null == newCamera) return;

            needsNewRails = true;

            // Add to Cache
            if (cameraLocations.TryGetValue(userName, out FixedSizedQueue<Camera> previousCameras))
            {
                previousCameras.Enqueue(newCamera);
            }
            else
            {
                FixedSizedQueue<Camera> newStack = new FixedSizedQueue<Camera>(MAX_CAMERAS_IN_QUEUE);
                newStack.Enqueue(newCamera);
                cameraLocations.Add(userName, newStack);
            }

            crashDoc.Redraw();
        }

        // What about queue?
        internal void FollowCamera()
        {
            User? toFollow = CrashDoc.ActiveDoc.Users.Where(u => u.Camera == CameraState.Follow).FirstOrDefault();
            if (string.IsNullOrEmpty(toFollow?.Name)) return;

            double param = 0;
            while(true)
            {
                if (!cameraLocations.TryGetValue(toFollow?.Name, out var currentCameras)) break;

                if (needsNewRails)
                {
                    setCurrentCameraRails(currentCameras);
                    param = 0;
                }

                PositionCamera(param);

                param += 1 / (FOLLOW_DELAY / MAX_CAMERAS_IN_QUEUE);

                Thread.Sleep(1000 / FPS);
            }
        }

        private void PositionCamera(double parameter)
        {
            Point3d cameraLocation = cameraLocationRail.PointAtNormalizedLength(parameter);
            Point3d cameraTarget = cameraTargetRail.PointAtNormalizedLength(parameter);

            var activeView = crashDoc.HostRhinoDoc.Views.ActiveView;
            activeView.ActiveViewport.SetCameraLocations(cameraTarget, cameraLocation);
        }

        private (Curve, Curve) constructInterpolatedPathFromCameras(IEnumerable<Camera> cameras)
        {
            // TODO: needs to take time into account
            Curve locationCurve = NurbsCurve.CreateInterpolatedCurve(cameras.Select(c => c.Location), 3);
            Curve targetCurve = NurbsCurve.CreateInterpolatedCurve(cameras.Select(c => c.Target), 3);

            return (locationCurve, targetCurve);
        }

        private void setCurrentCameraRails(IEnumerable<Camera> cameras)
        {
            var rails = constructInterpolatedPathFromCameras(cameras);

            cameraLocationRail = rails.Item1;
            cameraTargetRail = rails.Item2;
        }

        internal Dictionary<string, Camera> GetActiveCameras()
        {
            return cameraLocations.ToDictionary(cl => cl.Key, cl => cl.Value.FirstOrDefault());
        }

        public IEnumerator<Camera> GetEnumerator() => cameraLocations.Values.SelectMany(c => c).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }

}
