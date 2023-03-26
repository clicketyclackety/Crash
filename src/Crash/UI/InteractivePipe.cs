using System.Drawing;

using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.View;
using Crash.Handlers;
using Crash.Handlers.Plugins.Schemas;

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

		private static Dictionary<Type, Action<IChange, DrawEventArgs, DisplayMaterial>> DrawRegistry;

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
			Active = this;
			DrawRegistry = new Dictionary<Type, Action<IChange, DrawEventArgs, DisplayMaterial>>();
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

		DisplayMaterial cachedMaterial = new DisplayMaterial(Color.Blue);
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

			foreach (IChange Change in caches)
			{
				if (e.Display.InterruptDrawing()) return;

				if (!DrawRegistry.TryGetValue(Change.GetType(), out Action<IChange, DrawEventArgs, DisplayMaterial> func))
					continue;

				User user = doc.Users.Get(Change.Owner);

				func.Invoke(Change, e, new DisplayMaterial(user.Color));
				UpdateBoundingBox(Change as GeometryChange);
			}


			Dictionary<User, Camera> ActiveCameras = CrashDocRegistry.ActiveDoc.Cameras.GetActiveCameras();

			if (!DrawRegistry.TryGetValue(typeof(CameraChange), out var drawFunc)) return;
			foreach (var activeCamera in ActiveCameras)
			{
				if (e.Display.InterruptDrawing()) return;

				User user = activeCamera.Key;
				if (user.Camera != CameraState.Visible) continue;

				// TODO : Re-enable
				// drawFunc.Invoke(e, activeCamera, user.Color);
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

		internal static void RegisterDrawFunction(ChangeConverter<IChange> changeConverter)
		{
			DrawRegistry.Add(changeConverter.ChangeType, changeConverter.DrawChange);
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

	}

}
