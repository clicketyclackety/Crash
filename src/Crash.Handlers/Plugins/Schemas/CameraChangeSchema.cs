using Crash.Common.Changes;
using Crash.Common.Document;

using Rhino.Display;
using Rhino.Geometry;

namespace Crash.Handlers.Plugins.Schemas
{

	public sealed class CameraChangeSchema : ChangeConverter<CameraChange>
	{

		public CameraChangeSchema() : base(_getCameraChangeArgs())
		{
			RegisterChangeAction((int)ChangeAction.Camera, _cameraChangeAsync);
		}

		private async Task _cameraChangeAsync(CrashDoc crashDoc, IChange change)
		{
			var cameraChange = new CameraChange(change);
			crashDoc.Cameras.TryAddCamera(cameraChange);
			await Task.CompletedTask;
		}

		private static CustomChangeArgs _getCameraChangeArgs()
			=> CustomChangeArgs.Create<CameraChange>(
				_drawCameraArgs,
				_getCameraBounds);

		private static BoundingBox _getCameraBounds(IChange change)
		{
			if (change is not CameraChange cameraChange) return BoundingBox.Empty;

			var p1 = cameraChange.Camera.Target.ToRhino();
			var p2 = cameraChange.Camera.Location.ToRhino();

			return new BoundingBox(p1, p2);
		}

		const double width = 1000;
		const double height = 500;
		const int thickness = 4;
		private static void _drawCameraArgs(IChange change, DrawEventArgs e, DisplayMaterial material)
		{
			if (change is not CameraChange cameraChange) return;
			var camera = cameraChange.Camera;
			double ratio = 10;
			var length = 5000 * ratio;

			var location = camera.Location.ToRhino();
			var target = camera.Target.ToRhino();
			var viewAngle = target - location;

			var viewLine = new Line(camera.Location.ToRhino(), viewAngle, length);

			var cameraFrustrum = new Plane(viewLine.To, viewAngle);
			// Interval heightInterval = new Interval(-width * ratio / 2, width * ratio / 2);
			var widthInterval = new Interval(-height * ratio / 2, height * ratio / 2);
			var rectangle = new Rectangle3d(cameraFrustrum, widthInterval, widthInterval);

			var lines = new List<Line>(8);
			lines.AddRange(rectangle.ToPolyline().GetSegments());
			lines.Add(new Line(location, rectangle.PointAt(0)));
			lines.Add(new Line(location, rectangle.PointAt(1)));
			lines.Add(new Line(location, rectangle.PointAt(2)));
			lines.Add(new Line(location, rectangle.PointAt(3)));

			foreach (var line in lines)
			{
				e.Display.DrawPatternedLine(line, material.Diffuse, 0x00001111, thickness);
			}
		}

	}

}
