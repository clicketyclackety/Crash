using System.Drawing;

using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Events;
using Crash.Utils;

using Rhino.Display;
using Rhino.DocObjects;
using Rhino.Geometry;


namespace Crash.Handlers.Plugins
{
	public sealed class CrashGeometryChange : ChangeSchema<GeometryChange>
	{

		CrashGeometryChange()
		{
			RegisterChangeAction((int)ChangeAction.Add, _addChangeAsync);
			RegisterChangeAction((int)ChangeAction.Remove, _removeChangeAsync);

			RegisterChangeAction((int)ChangeAction.Lock, _lockChangeAsync);
			RegisterChangeAction((int)ChangeAction.Unlock, _unlockChangeAsync);

			RegisterCustomChange(_getGeometryChangeArgs());
		}

		#region Custom Change Geometry Change

		private CustomChangeArgs<GeometryChange> _getGeometryChangeArgs()
		{
			var changeArgs = new CustomChangeArgs<GeometryChange>()
			{
				AddAction = _addGeometryChange,
				RemoveAction = _removeGeometryChange,
				DrawArgs = _drawGeometryArgs,
				GetBoundingBox = _getGeometryBounds
			};

			return changeArgs;
		}

		private BoundingBox _getGeometryBounds(GeometryChange change)
			=> change.Geometry.GetBoundingBox(false);

		private void _addGeometryChange(GeometryChange change, CrashDoc crashDoc)
		{
			ChangeAction changeAction = (ChangeAction)change.Action;

			if (!changeAction.HasFlag(ChangeAction.Temporary))
			{
				var rhinoDoc = CrashDocRegistry.GetRelatedDocument(crashDoc);

				Guid id = rhinoDoc.Objects.Add(change.Geometry);
				RhinoObject rObj = rhinoDoc.Objects.FindId(id);
				ChangeUtils.SyncHost(rObj, change);
			}

			crashDoc.CacheTable.UpdateChangeAsync(change);
		}

		private void _removeGeometryChange(GeometryChange change, CrashDoc crashDoc)
		{
			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(crashDoc);

			rhinoDoc.Objects.Delete(change.RhinoId, true);
			crashDoc.CacheTable.RemoveChange(change.Id);
		}

		private void _drawGeometryArgs(GeometryChange change, DrawEventArgs e, DisplayMaterial material)
		{
			var geom = change.Geometry;
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

		private async Task _addChangeAsync(CrashDoc crashDoc, IChange change)
		{
			GeometryChange geomChange = new GeometryChange(change);
			ChangeArgs changeArgs = new ChangeArgs(crashDoc, geomChange);
			IdleAction bakeAction = new IdleAction(geomChange.AddToDocument, changeArgs);
			crashDoc.Queue.AddAction(bakeAction);

			await Task.CompletedTask;
		}

		private async Task _removeChangeAsync(CrashDoc crashDoc, IChange change)
		{
			GeometryChange geomChange = new GeometryChange(new Change(change.Id, null, null));
			ChangeArgs changeArgs = new ChangeArgs(crashDoc, geomChange);
			IdleAction bakeAction = new IdleAction(geomChange.RemoveFromDocument, changeArgs);
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
