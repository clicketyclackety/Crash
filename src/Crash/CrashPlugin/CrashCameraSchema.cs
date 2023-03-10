using Crash.Common.Changes;
using Crash.Common.Document;

using Rhino.Display;
using Rhino.Geometry;

namespace Crash.Handlers.Plugins
{

	public sealed class CrashCameraChange : ChangeSchema<CameraChange>
	{

		CrashCameraChange()
		{
			RegisterChangeAction((int)ChangeAction.Camera, _cameraChangeAsync);
			RegisterCustomChange(_getCameraChangeArgs());
		}

		private async Task _cameraChangeAsync(CrashDoc crashDoc, IChange change)
		{
			CameraChange cameraChange = new CameraChange(change);
			crashDoc.Cameras.TryAddCamera(cameraChange);
			await Task.CompletedTask;
		}

		private CustomChangeArgs<CameraChange> _getCameraChangeArgs()
		{
			var cameraArgs = new CustomChangeArgs<CameraChange>()
			{
				AddAction = _addCameraChange,
				DrawArgs = _drawCameraArgs,
				GetBoundingBox = _getCameraBounds
			};

			return cameraArgs;
		}

		private BoundingBox _getCameraBounds(CameraChange change)
		{
			Point3d p1 = change.Camera.Target.ToRhino();
			Point3d p2 = change.Camera.Location.ToRhino();
			return new BoundingBox(p1, p2);
		}

		private void _addCameraChange(CameraChange camera, CrashDoc crashDoc)
		{
			ChangeAction changeAction = (ChangeAction)camera.Action;

			if (!changeAction.HasFlag(ChangeAction.Temporary))
			{
				crashDoc.Cameras.TryAddCamera(camera);
			}
		}

		const double width = 1000;
		const double height = 500;
		const int thickness = 4;
		private void _drawCameraArgs(CameraChange change, DrawEventArgs e, DisplayMaterial material)
		{
			var camera = change.Camera;
			double ratio = 10;
			double length = 5000 * ratio;

			Point3d location = camera.Location.ToRhino();
			Point3d target = camera.Target.ToRhino();
			Vector3d viewAngle = target - location;

			Line viewLine = new Line(camera.Location.ToRhino(), viewAngle, length);

			Plane cameraFrustrum = new Plane(viewLine.To, viewAngle);
			// Interval heightInterval = new Interval(-width * ratio / 2, width * ratio / 2);
			Interval widthInterval = new Interval(-height * ratio / 2, height * ratio / 2);
			Rectangle3d rectangle = new Rectangle3d(cameraFrustrum, widthInterval, widthInterval);

			List<Line> lines = new List<Line>(8);
			lines.AddRange(rectangle.ToPolyline().GetSegments());
			lines.Add(new Line(location, rectangle.PointAt(0)));
			lines.Add(new Line(location, rectangle.PointAt(1)));
			lines.Add(new Line(location, rectangle.PointAt(2)));
			lines.Add(new Line(location, rectangle.PointAt(3)));

			foreach (Line line in lines)
			{
				e.Display.DrawPatternedLine(line, material.Diffuse, 0x00001111, thickness);
			}
		}

	}

}
