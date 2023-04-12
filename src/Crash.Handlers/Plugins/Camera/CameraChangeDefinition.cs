using Crash.Common.Changes;
using Crash.Handlers.Plugins.Camera.Create;
using Crash.Handlers.Plugins.Camera.Recieve;

using Rhino.Display;
using Rhino.Geometry;

namespace Crash.Handlers.Plugins.Camera
{

	public sealed class CameraChangeDefinition : IChangeDefinition
	{

		public Type ChangeType => typeof(CameraChange);

		public string ChangeName => $"{nameof(Crash)}.{nameof(CameraChange)}";

		public IEnumerable<IChangeCreateAction> CreateActions { get; }

		public IEnumerable<IChangeRecieveAction> RecieveActions { get; }


		public CameraChangeDefinition()
		{
			CreateActions = new List<IChangeCreateAction>
			{
				new CameraCreateAction()
			};
			RecieveActions = new List<IChangeRecieveAction>
			{
				new CameraRecieveAction()
			};
		}

		const double width = 1000;
		const double height = 500;
		const int thickness = 4;
		public void Draw(DrawEventArgs drawArgs, DisplayMaterial material, IChange change)
		{
			if (change is not CameraChange cameraChange) return;

			Active = new CameraGraphic(cameraChange.Camera);
			Active.Draw(drawArgs, material);
		}

		public BoundingBox GetBoundingBox(IChange change)
		{
			BoundingBox bounds = BoundingBox.Empty;
			if (null == Active.lines || Active.lines.Count == 0)
				return bounds;

			bounds = new BoundingBox(Active.lines.Select(l => l.From));
			bounds.Inflate(1.25);

			return bounds;
		}

		CameraGraphic Active;

		private struct CameraGraphic
		{
			double ratio = 10;
			double length => 1000 * ratio;

			internal readonly List<Line> lines;

			internal CameraGraphic(Crash.Common.View.Camera camera)
			{
				var location = camera.Location.ToRhino();
				var target = camera.Target.ToRhino();
				var viewAngle = target - location;

				var viewLine = new Line(camera.Location.ToRhino(), viewAngle, length);

				var cameraFrustrum = new Plane(viewLine.To, viewAngle);
				Point3d xPoint = viewLine.To;
				xPoint.Transform(Transform.Translation(new Vector3d(100, 0, 0)));
				double rads = Vector3d.VectorAngle(cameraFrustrum.XAxis, Vector3d.XAxis, cameraFrustrum);
				cameraFrustrum.Rotate(rads, cameraFrustrum.ZAxis);

				// Interval heightInterval = new Interval(-width * ratio / 2, width * ratio / 2);
				var widthInterval = new Interval(-height * ratio / 2, height * ratio / 2);
				var rectangle = new Rectangle3d(cameraFrustrum, widthInterval, widthInterval);

				lines = new List<Line>(8);
				lines.AddRange(rectangle.ToPolyline().GetSegments());
				lines.Add(new Line(location, rectangle.PointAt(0)));
				lines.Add(new Line(location, rectangle.PointAt(1)));
				lines.Add(new Line(location, rectangle.PointAt(2)));
				lines.Add(new Line(location, rectangle.PointAt(3)));
			}

			public void Draw(DrawEventArgs drawArgs, DisplayMaterial material)
			{
				foreach (var line in lines)
				{
					drawArgs.Display.DrawPatternedLine(line, material.Diffuse, 0x00001111, thickness);
				}
			}

		}

	}

}
