using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Tables;
using Crash.Common.View;

using Rhino;
using Rhino.Display;
using Rhino.Geometry;

namespace Crash.Handlers
{

	/// <summary>Handles Camera Objects and movement</summary>
	public static class CameraOperator
	{
		private const int MINIMUM_DISPLACEMENT = 1000; // What unit?
		private const int CHANGE_DELAY = 200; // in milliseconds
		private const int FOLLOW_DELAY = 750; // in milliseconds
		private const int FPS = 30; // TODO: Make this a setting

		private static Curve cameraLocationRail;
		private static Curve cameraTargetRail;

		private static DateTime lastChange = DateTime.Now;
		private static Point3d lastLocation = Point3d.Unset;
		private static Point3d lastTarget = Point3d.Unset;


		static CameraOperator()
		{

		}

		// Intentionally Static
		internal static void RhinoView_Modified(object sender, ViewEventArgs v)
		{
			if (null == CrashDocRegistry.ActiveDoc?.LocalClient) return;
			if (CrashDocRegistry.ActiveDoc.Users.Any(u => u.Camera == CameraState.Follow)) return;

			RhinoView view = v.View;
			Point3d cameraLocation = view.ActiveViewport.CameraLocation;
			Point3d cameraTarget = view.ActiveViewport.CameraTarget;
			var currentChange = DateTime.UtcNow;

			if ((currentChange - lastChange).TotalMilliseconds < CHANGE_DELAY) return;

			// Limit the number of Changes we send
			if (cameraLocation.DistanceTo(lastLocation) < MINIMUM_DISPLACEMENT ||
				cameraTarget.DistanceTo(lastTarget) < MINIMUM_DISPLACEMENT)
			{
				lastChange = currentChange;

				var camera = new Camera(cameraLocation.ToCrash(), cameraTarget.ToCrash());
				var cameraChange = CameraChange.CreateNew(camera, CrashDocRegistry.ActiveDoc.Users.CurrentUser.Name);
				var serverChange = new Change(cameraChange);

				Task.Run(() => CrashDocRegistry.ActiveDoc.LocalClient.CameraChangeAsync(serverChange));
			}
		}

		private static (Curve, Curve) constructInterpolatedPathFromCameras(IEnumerable<Camera> cameras)
		{
			// TODO: needs to take time into account
			Curve locationCurve = NurbsCurve.CreateInterpolatedCurve(cameras.Select(c => c.Location).ToRhino(), 3);
			Curve targetCurve = NurbsCurve.CreateInterpolatedCurve(cameras.Select(c => c.Target).ToRhino(), 3);

			return (locationCurve, targetCurve);
		}

		private static void setCurrentCameraRails(IEnumerable<Camera> cameras)
		{
			var rails = constructInterpolatedPathFromCameras(cameras);

			cameraLocationRail = rails.Item1;
			cameraTargetRail = rails.Item2;
		}

		// What about queue?
		public static void FollowCamera()
		{
			var crashDoc = CrashDocRegistry.ActiveDoc;
			if (null == crashDoc) return;

			var toFollow = crashDoc.Users.Where(u => u.Camera == CameraState.Follow).FirstOrDefault();
			if (string.IsNullOrEmpty(toFollow.Name)) return;

			double param = 0;
			while (true)
			{
				if (!crashDoc.Cameras.TryGetCamera(toFollow, out var currentCameras)) break;

				if (crashDoc.Cameras.CameraIsInvalid)
				{
					setCurrentCameraRails(currentCameras);
					param = 0;
				}

				PositionCamera(param);

				param += 1 / (FOLLOW_DELAY / CameraTable.MAX_CAMERAS_IN_QUEUE);

				Thread.Sleep(1000 / FPS);
			}
		}


		private static void PositionCamera(double parameter)
		{
			Point3d cameraLocation = cameraLocationRail.PointAtNormalizedLength(parameter);
			Point3d cameraTarget = cameraTargetRail.PointAtNormalizedLength(parameter);

			var activeView = RhinoDoc.ActiveDoc.Views.ActiveView;
			activeView.ActiveViewport.SetCameraLocations(cameraTarget, cameraLocation);
		}

	}
}
