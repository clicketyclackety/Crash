using Crash.Common.Changes;
using Crash.Handlers.Plugins.Camera.Create;
using Crash.Handlers.Plugins.Camera.Recieve;

using Rhino.Display;
using Rhino.Geometry;

namespace Crash.Handlers.Plugins.Camera
{

	/// <summary>Defines the Camera Change Type</summary>
	public sealed class CameraChangeDefinition : IChangeDefinition
	{
		/// <inheritdoc/>
		public Type ChangeType => typeof(CameraChange);

		/// <inheritdoc/>
		public string ChangeName => CameraChange.ChangeName;

		/// <inheritdoc/>
		public IEnumerable<IChangeCreateAction> CreateActions { get; }

		/// <inheritdoc/>
		public IEnumerable<IChangeRecieveAction> RecieveActions { get; }

		/// <summary>Constructs the Definition</summary>
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


		/// <inheritdoc/>
		public void Draw(DrawEventArgs drawArgs, DisplayMaterial material, IChange change)
		{
			if (change is not CameraChange cameraChange) return;

			Active = new CameraGraphic(cameraChange.Camera);
			Active.Draw(drawArgs, material);
		}

		/// <inheritdoc/>
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

		/// <summary>The Camera Graphic for the Pipeline</summary>
		private struct CameraGraphic
		{

			internal readonly List<Line> lines;
			internal readonly Plane plane;
			const double Length = 3000;
			const double Width = 1920;
			const double Height = 1080;
			const double CrossHairLength = 80;

			static Interval WidthInterval = new Interval(-Width / 2, Width / 2);
			static Interval HeightInterval = new Interval(-Height / 2, Height / 2);

			internal CameraGraphic(Crash.Common.View.Camera camera)
			{
				var location = camera.Location.ToRhino();
				var target = camera.Target.ToRhino();
				var normal = target - location;

				var viewLine = new Line(location, normal, Length);

				var plane = GetCameraPlane(viewLine, normal);
				var rectangle = new Rectangle3d(plane, HeightInterval, WidthInterval);

				lines = new List<Line>(12);
				lines.AddRange(rectangle.ToPolyline().GetSegments());
				lines.Add(new Line(location, rectangle.PointAt(0)));
				lines.Add(new Line(location, rectangle.PointAt(1)));
				lines.Add(new Line(location, rectangle.PointAt(2)));
				lines.Add(new Line(location, rectangle.PointAt(3)));

				Vector3d xVec = plane.XAxis;
				Vector3d yVec = plane.YAxis;
				var l1 = new Line(plane.Origin, xVec * CrossHairLength);
				var l2 = new Line(plane.Origin, xVec * -CrossHairLength);
				var l3 = new Line(plane.Origin, yVec * CrossHairLength);
				var l4 = new Line(plane.Origin, yVec * -CrossHairLength);

				lines.Add(l1);
				lines.Add(l2);
				lines.Add(l3);
				lines.Add(l4);
			}

			private Plane GetCameraPlane(Line viewLine, Vector3d normal)
			{
				Point3d origin = viewLine.To;
				var cameraFrustrum = new Plane(origin, normal);
				Circle circ = new Circle(cameraFrustrum, 100);

				Point3d xPoint = origin;
				xPoint.Transform(Transform.Translation(new Vector3d(0, 0, -100)));
				circ.ClosestParameter(xPoint, out double xParam);
				xPoint = circ.PointAt(xParam);

				double quarter = getUnParameterised(circ.Circumference, 0.25);
				double yParam = xParam - quarter;
				Point3d yPoint = circ.PointAt(yParam);

				Plane plane = new Plane(origin, xPoint, yPoint);
				return plane;
			}

			private double getReparameterised(double length, double param)
				=> param / length;

			private double getUnParameterised(double length, double param)
				=> length * param;

			public void Draw(DrawEventArgs drawArgs, DisplayMaterial material)
			{
				foreach (var line in lines)
				{
					drawArgs.Display.DrawPatternedLine(line, material.Diffuse, 0x11111111, 8);
				}
			}

		}

	}

}
