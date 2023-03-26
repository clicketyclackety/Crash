using System.Drawing;

using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Events;
using Crash.Handlers.Args;

using Rhino;
using Rhino.Display;
using Rhino.Geometry;


namespace Crash.Handlers.Plugins.Schemas
{
	public sealed class GeometryChangeSchema : ChangeConverter<GeometryChange>
	{

		public GeometryChangeSchema() : base(_getGeometryChangeArgs())
		{
			RegisterChangeAction((int)ChangeAction.Add, _addChangeAsync);
			RegisterChangeAction((int)ChangeAction.Remove, _removeChangeAsync);

			RegisterChangeAction((int)ChangeAction.Lock, _lockChangeAsync);
			RegisterChangeAction((int)ChangeAction.Unlock, _unlockChangeAsync);
		}

		#region Custom Change Geometry Change

		private static BoundingBox _getGeometryBounds(IChange change)
		{
			if (change is not GeometryChange geomChange) return BoundingBox.Empty;
			return geomChange.Geometry.GetBoundingBox(false);
		}


		private static void _drawGeometryArgs(IChange change, DrawEventArgs e, DisplayMaterial material)
		{
			if (change is not GeometryChange geomChange) return;
			var geom = geomChange.Geometry;
			if (geom is Curve cv)
			{
				e.Display.DrawCurve(cv, material.Diffuse, 2);
			}
			else if (geom is Brep brep)
			{
				e.Display.DrawBrepShaded(brep, material);
			}
			else if (geom is Mesh mesh)
			{
				e.Display.DrawMeshShaded(mesh, material);
			}
			else if (geom is Extrusion ext)
			{
				e.Display.DrawExtrusionWires(ext, material.Diffuse);
			}
			else if (geom is TextEntity te)
			{
				e.Display.DrawText(te, material.Diffuse);
			}
			else if (geom is TextDot td)
			{
				e.Display.DrawDot(td, Color.White, material.Diffuse, material.Diffuse);
			}
			else if (geom is Surface srf)
			{
				e.Display.DrawSurface(srf, material.Diffuse, 1);
			}
			else if (geom is Rhino.Geometry.Point pnt)
			{
				e.Display.DrawPoint(pnt.Location, material.Diffuse);
			}
			else if (geom is AnnotationBase ab)
			{
				e.Display.DrawAnnotation(ab, material.Diffuse);
			}
		}

		#endregion

		#region ChangeAction Actions

		private void AddToDocument(CrashEventArgs args)
		{
			if (args is not ChangeArgs changeArgs) return;
			RhinoDoc rhinoDoc = CrashDocRegistry.GetRelatedDocument(changeArgs.CrashDoc);
			rhinoDoc.Objects.Add(changeArgs.Change.Geometry);

			var action = (ChangeAction)changeArgs.Change.Action;
			if (action.HasFlag(ChangeAction.Temporary))
			{
				changeArgs.CrashDoc.CacheTable.UpdateChangeAsync(changeArgs.Change);
			}
		}

		private async Task _addChangeAsync(CrashDoc crashDoc, IChange change)
		{
			var geomChange = new GeometryChange(change);
			var changeArgs = new ChangeArgs(crashDoc, geomChange);
			var bakeAction = new IdleAction(AddToDocument, changeArgs);
			crashDoc.Queue.AddAction(bakeAction);

			await Task.CompletedTask;
		}

		private void RemoveFromDocument(CrashEventArgs args)
		{
			if (args is not ChangeArgs changeArgs) return;
			RhinoDoc rhinoDoc = CrashDocRegistry.GetRelatedDocument(changeArgs.CrashDoc);
			rhinoDoc.Objects.Delete(changeArgs.Change.RhinoId, true);
			changeArgs.CrashDoc.CacheTable.RemoveChange(changeArgs.Change.Id);
		}

		private async Task _removeChangeAsync(CrashDoc crashDoc, IChange change)
		{
			var geomChange = new GeometryChange(new Change(change.Id, null, null));
			var changeArgs = new ChangeArgs(crashDoc, geomChange);
			var bakeAction = new IdleAction(RemoveFromDocument, changeArgs);
			crashDoc.Queue.AddAction(bakeAction);

			await Task.CompletedTask;
		}

		private async Task _unlockChangeAsync(CrashDoc crashDoc, IChange change)
		{
			var rDoc = CrashDocRegistry.GetRelatedDocument(crashDoc);
			if (rDoc is not object) return;

			if (crashDoc.CacheTable.TryGetValue(change.Id, out GeometryChange cachedChange))
			{
				rDoc.Objects.Unlock(cachedChange.RhinoId, true);
			}

			await Task.CompletedTask;
		}

		private async Task _lockChangeAsync(CrashDoc crashDoc, IChange change)
		{
			var rDoc = CrashDocRegistry.GetRelatedDocument(crashDoc);
			if (rDoc is not object) return;

			if (crashDoc.CacheTable.TryGetValue(change.Id, out GeometryChange cachedChange))
			{
				rDoc.Objects.Lock(cachedChange.RhinoId, true);
			}

			await Task.CompletedTask;
		}

		#endregion

	}

}
