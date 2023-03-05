using System.Drawing;

using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.View;
using Crash.Handlers;

using Rhino.Display;
using Rhino.Geometry;

namespace Crash.UI
{

	/// <summary>
	/// Interactive pipeline for crash geometry display
	/// </summary>
	// TODO : Make this static, and turn it into a template that just
	// grabs things from the CrashDoc
	// There's no need to recreate the same class again and again
	// and store so much geometry.
	internal sealed class InteractivePipe : IDisposable
	{

		double scale => RhinoDoc.ActiveDoc is object ? RhinoMath.UnitScale(UnitSystem.Meters, RhinoDoc.ActiveDoc.ModelUnitSystem) : 0;
		private int FAR_AWAY => (int)scale * 1_5000;
		private int VERY_FAR_AWAY => (int)scale * 7_5000;

		// TODO : Does this ever get shrunk? It should do.
		// TODO : Don't draw things not in the view port
		private BoundingBox bbox;

		private bool enabled { get; set; }

		/// <summary>
		/// Pipeline enabled, disabling hides it
		/// </summary>
		internal bool Enabled
		{
			get => enabled;
			set
			{
				if (enabled == value) return;

				enabled = value;

				if (enabled)
				{
					DisplayPipeline.CalculateBoundingBox += CalculateBoundingBox;
					DisplayPipeline.PostDrawObjects += PostDrawObjects;
				}
				else
				{
					DisplayPipeline.CalculateBoundingBox -= CalculateBoundingBox;
					DisplayPipeline.PostDrawObjects -= PostDrawObjects;
				}
			}
		}

		internal static InteractivePipe Active;

		/// <summary>
		/// Empty constructor
		/// </summary>
		internal InteractivePipe()
		{
			bbox = new BoundingBox(-100, -100, -100, 100, 100, 100);
			PipeCameraCache = new Dictionary<Color, Line[]>();
			Active = this;
		}

		/// <summary>
		/// Method to calculate the bounding box
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void CalculateBoundingBox(object sender, CalculateBoundingBoxEventArgs e)
		{
			e.IncludeBoundingBox(bbox);
		}

		/// <summary>
		/// Post draw object events
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void PostDrawObjects(object sender, DrawEventArgs e)
		{
			if (null == CrashDocRegistry.ActiveDoc?.CacheTable) return;
			if (null == CrashDocRegistry.ActiveDoc?.Cameras) return;
			if (null == CrashDocRegistry.ActiveDoc?.Users) return;

			// bbox = new BoundingBox(-100, -100, -100, 100, 100, 100);

			var enumer = CrashDocRegistry.ActiveDoc.CacheTable.GetEnumerator<GeometryChange>();

			while (enumer.MoveNext())
			{
				if (e.Display.InterruptDrawing()) return;

				GeometryChange Change = enumer.Current;

				User user = CrashDocRegistry.ActiveDoc.Users.Get(Change.Owner);
				if (user.Visible != true) continue;

				DrawChange(e, Change, user.Color);
				UpdateBoundingBox(Change);
			}

			Dictionary<User, Camera> ActiveCameras = CrashDocRegistry.ActiveDoc.Cameras.GetActiveCameras();
			foreach (var activeCamera in ActiveCameras)
			{
				if (e.Display.InterruptDrawing()) return;

				User user = activeCamera.Key;
				if (user.Camera != CameraState.Visible) continue;

				DrawCamera(e, activeCamera.Value, user.Color);
			}
		}

		private Dictionary<Color, Line[]> PipeCameraCache;

		const double width = 1000;
		const double height = 500;
		const int thickness = 4;
		private void DrawCamera(DrawEventArgs e, Camera camera, Color userColor)
		{
			PipeCameraCache.Remove(userColor);
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

			Interval zInterval = new Interval(0, length);
			BoundingBox cameraBox = new Box(cameraFrustrum, widthInterval, widthInterval, zInterval).BoundingBox;
			bbox.Union(cameraBox);

			foreach (Line line in lines)
			{
				// e.Display.DrawLines(lines, userColor, 3);
				e.Display.DrawPatternedLine(line, userColor, 0x00001111, thickness);
			}
		}

		/// <summary>
		/// Updates the BoundingBox of the Pipeline
		/// </summary>
		/// <param name="Change"></param>
		private void UpdateBoundingBox(GeometryChange Change)
		{
			GeometryBase? geom = Change.Geometry;
			if (geom is object)
			{
				BoundingBox ChangeBox = geom.GetBoundingBox(false);
				ChangeBox.Inflate(1.25);
				bbox.Union(ChangeBox);
			}

		}

		/// Re-using materials is much faster
		DisplayMaterial cachedMaterial = new DisplayMaterial(Color.Blue);
		/// <summary>
		/// Draws a Change in the pipeline.
		/// </summary>
		/// <param name="e">The EventArgs from the DisplayConduit</param>
		/// <param name="Change">The Change</param>
		/// <param name="color">The colour for the Change, based on the user.</param>
		private void DrawChange(DrawEventArgs e, GeometryChange Change, Color color)
		{
			GeometryBase? geom = Change.Geometry;
			if (geom == null) return;

			User user = CrashDocRegistry.ActiveDoc.Users.Get(Change.Owner);
			if (!user.Visible) return;

			if (cachedMaterial.Diffuse != color)
			{
				cachedMaterial = new DisplayMaterial(color);
			}

			BoundingBox bbox = Change.Geometry.GetBoundingBox(false);
			double distanceTo = e.Viewport.CameraLocation.DistanceTo(bbox.Center);

			// Over a certain size, no need to draw either. BBox relative to distance is important.

			if (distanceTo > VERY_FAR_AWAY)
			{
				return;
			}
			if (distanceTo > FAR_AWAY)
			{
				e.Display.DrawBox(bbox, color, 1);
				return;
			}

			try
			{
				if (geom is Curve cv)
				{
					e.Display.DrawCurve(cv, color, 2);
				}
				else if (geom is Brep brep)
				{
					e.Display.DrawBrepShaded(brep, cachedMaterial);
				}
				else if (geom is Mesh mesh)
				{
					e.Display.DrawMeshShaded(mesh, cachedMaterial);
				}
				else if (geom is Extrusion ext)
				{
					e.Display.DrawExtrusionWires(ext, cachedMaterial.Diffuse);
				}
				else if (geom is TextEntity te)
				{
					e.Display.DrawText(te, cachedMaterial.Diffuse);
				}
				else if (geom is TextDot td)
				{
					e.Display.DrawDot(td, Color.White, cachedMaterial.Diffuse, cachedMaterial.Diffuse);
				}
				else if (geom is Surface srf)
				{
					e.Display.DrawSurface(srf, cachedMaterial.Diffuse, 1);
				}
				else if (geom is Rhino.Geometry.Point pnt)
				{
					e.Display.DrawPoint(pnt.Location, cachedMaterial.Diffuse);
				}
				else if (geom is AnnotationBase ab)
				{
					e.Display.DrawAnnotation(ab, cachedMaterial.Diffuse);
				}
				else
				{
					;
				}
			}
			catch (Exception)
			{

			}
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

	}

}
