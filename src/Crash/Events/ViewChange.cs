using Rhino.Display;
using Rhino.Geometry;

// https://developer.rhino3d.com/api/RhinoCommon/html/P_Rhino_Display_RhinoViewport_ChangeCounter.htm
// Worth a Read.

namespace Crash.Events
{
    /// <summary>
    /// Add item event handler
    /// </summary>
    internal static class ViewChange
    {

        private const double MINIMUM_DISPLACEMENT = 100; // What unit?
        private const double CHANGE_DELAY = 250; // in MS

        private static DateTime lastChange = DateTime.MinValue;
        private static Point3d lastLocation = Point3d.Unset;
        private static Point3d lastTarget = Point3d.Unset;


        internal static void Event(object sender, ViewEventArgs v)
        {
            if (null == Document.CrashDoc.ActiveDoc) return;
            if (Tables.UserTable.CurrentUser.Camera == CameraState.Follow) return;

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

                Camera camera = new Camera(cameraLocation, cameraTarget, Tables.UserTable.CurrentUser);
                CameraSpeck cameraSpeck = CameraSpeck.CreateNew(camera);
                Speck serverSpeck = new Speck(cameraSpeck);

                ClientManager.LocalClient?.Add(serverSpeck);
            }
        }
    }
}