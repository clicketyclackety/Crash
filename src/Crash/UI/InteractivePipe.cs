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

		private Dictionary<Type, Action<DrawEventArgs, IChange, Color>> DrawRegistry;


		internal static InteractivePipe Active;

		/// <summary>
		/// Empty constructor
		/// </summary>
		internal InteractivePipe()
		{
			bbox = new BoundingBox(-100, -100, -100, 100, 100, 100);
			PipeCameraCache = new Dictionary<Color, Line[]>();
			Active = this;
			DrawRegistry = new Dictionary<Type, Action<DrawEventArgs, IChange, Color>>();

			DrawRegistry.Add(typeof(GeometryChange), DrawCrashChange);
			// DrawRegistry.Add(typeof(CameraChange), DrawCameraChange);
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

			var doc = CrashDocRegistry.ActiveDoc;
			string currentUser = doc.Users.CurrentUser.Name;
			var caches = CrashDocRegistry.ActiveDoc.CacheTable.GetChanges()
				.Where(c => c.Owner != currentUser)
				.Where(c => ((ChangeAction)c.Action).HasFlag(ChangeAction.Temporary))
				.OrderBy(c => doc.Users.Get(c.Owner).Color);

			foreach (ICachedChange Change in caches)
			{
				if (e.Display.InterruptDrawing()) return;

				if (!DrawRegistry.TryGetValue(Change.GetType(), out Action<DrawEventArgs, IChange, Color> func))
					continue;

				User user = doc.Users.Get(Change.Owner);

				func.Invoke(e, Change, user.Color);
				UpdateBoundingBox(Change as GeometryChange);
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
		private void DrawCrashChange(DrawEventArgs e, IChange Change, Color color)
		{
			GeometryBase? geom = (Change as GeometryChange).Geometry;
			if (geom == null) return;

			if (cachedMaterial.Diffuse != color)
			{
				cachedMaterial = new DisplayMaterial(color);
			}

			BoundingBox bbox = geom.GetBoundingBox(false);
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
				// Call Draw args
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
