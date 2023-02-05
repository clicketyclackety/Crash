using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Rhino.Display;
using Rhino.Geometry;
using Crash.Events;
using Crash.Document;

namespace Crash.Utilities
{

    internal sealed class CameraCache
    {
        private const double MINIMUM_DISPLACEMENT = 1000; // What unit?
        private const double CHANGE_DELAY = 500; // in milliseconds
        private const double FOLLOW_DELAY = 1000; // in milliseconds

        private static DateTime lastChange = DateTime.MinValue;
        private static Point3d lastLocation = Point3d.Unset;
        private static Point3d lastTarget = Point3d.Unset;

        private static ConcurrentDictionary<string, Stack<Camera>> cameraLocations;

        static CameraCache()
        {
            cameraLocations = new ConcurrentDictionary<string, Stack<Camera>>();
        }


        internal static void RhinoView_Modified(object sender, Rhino.Display.ViewEventArgs v)
        {
            if (null == Document.CrashDoc.ActiveDoc) return;
            if (Document.CrashDoc.ActiveDoc.Users.Any(u => u.Camera == CameraState.Follow)) return;

            RhinoView view = v.View;
            Point3d cameraLocation = view.ActiveViewport.CameraLocation;
            Point3d cameraTarget = view.ActiveViewport.CameraTarget;
            DateTime currentChange = DateTime.UtcNow;

            // Limit the number of Specks we send
            if ((currentChange - lastChange).TotalMilliseconds < CHANGE_DELAY ||
                cameraLocation.DistanceTo(lastLocation) < MINIMUM_DISPLACEMENT ||
                cameraTarget.DistanceTo(lastTarget) < MINIMUM_DISPLACEMENT)
            {
                lastChange = currentChange;

                Camera camera = new Camera(cameraLocation, cameraTarget);
                CameraSpeck cameraSpeck = CameraSpeck.CreateNew(camera);
                Speck serverSpeck = new Speck(cameraSpeck);

                ClientManager.LocalClient?.CameraChange(serverSpeck);
            }
        }

        internal static void OnCameraChange(string userName, Speck cameraSpeck)
        {
            User? user = Document.CrashDoc.ActiveDoc?.Users.Get(userName);
            if (null == user) return;
            Camera? newCamera = Camera.FromJSON(cameraSpeck.Payload);
            if (null == newCamera) return;

            // Add to Cache
            if (cameraLocations.TryGetValue(userName, out Stack<Camera> previousCameras))
            {
                previousCameras.Push(newCamera);
                cameraLocations.TryAdd(userName, previousCameras);
            }
            else
            {
                Stack<Camera> newStack = new Stack<Camera>();
                newStack.Push(newCamera);
                cameraLocations.TryAdd(userName, newStack);
            }

            if (user.Camera == CameraState.Follow)
            {
                EventManagement.currentQueue.AddAction(FollowCamera);
            }

            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }

        // What about queue?
        internal static void FollowCamera()
        {
            User toFollow = CrashDoc.ActiveDoc.Users.Where(u => u.Camera == CameraState.Follow).FirstOrDefault();
            if (null == toFollow) return;
            if (!cameraLocations.TryGetValue(toFollow?.Name, out Stack<Camera> cameras)) return;
            Camera camera = cameras.First();

            var activeView = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView;
            activeView.ActiveViewport.SetCameraLocations(camera.Target,camera.Location);
        }

        internal static Dictionary<string, Camera> GetActiveCameras()
        {
            return cameraLocations.ToDictionary(cl => cl.Key, cl => cl.Value.Peek());
        }



    }

}
