using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Utils;

using Rhino;

namespace Crash.Handlers
{
	// TODO : Make internal
	public sealed class CrashDocumentState : IDisposable
	{

		public CrashDoc Document;

		internal CrashDocumentState(CrashDoc document)
		{
			Document = document;
			RhinoApp.Idle += CallIdle;
			Document.Queue.OnCompletedQueue += Queue_OnCompletedQueue;
		}

		public override int GetHashCode() => Document.GetHashCode();

		private void CallIdle(object sender, EventArgs e)
		{
			Document.Queue.RunNextAction();
		}

		private void Queue_OnCompletedQueue(object sender, EventArgs e)
		{
			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(Document);
			rhinoDoc?.Views?.Redraw();
		}


		#region Events
		public void RegisterEvents()
		{
			RhinoDoc.AddRhinoObject += AddItemEvent;
			RhinoDoc.UndeleteRhinoObject += AddItemEvent;
			RhinoDoc.DeleteRhinoObject += RemoveItemEvent;
			RhinoDoc.SelectObjects += SelectItemEvent;
			RhinoDoc.DeselectObjects += SelectItemEvent;
			RhinoDoc.DeselectAllObjects += SelectAllItemsEvent;
		}

		private void AddItemEvent(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
		{
			if (null == Document?.CacheTable) return;
			if (Document.CacheTable.IsInit) return;
			if (Document.CacheTable.SomeoneIsDone) return;

			var userName = Document.Users?.CurrentUser.Name;
			if (string.IsNullOrEmpty(userName))
			{
				Console.WriteLine("Current User is null");
				return;
			}

			var Change = GeometryChange.CreateNew(userName, e.TheObject.Geometry);

			ChangeUtils.SyncHost(e.TheObject, Change);

			var serverChange = new Change(Change);
			Document?.LocalClient?.AddAsync(serverChange);
		}

		internal void RemoveItemEvent(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
		{
			if (!ChangeUtils.TryGetChangeId(e.TheObject, out Guid id)) return;

			Document.LocalClient.DeleteAsync(id);
		}

		private void SelectItemEvent(object sender, Rhino.DocObjects.RhinoObjectSelectionEventArgs e)
		{
			foreach (var rhinoObject in e.RhinoObjects)
			{
				if (rhinoObject.IsLocked)
					continue;

				if (!Utils.ChangeUtils.TryGetChangeId(rhinoObject, out Guid id)) continue;

				if (e.Selected)
					Document.LocalClient?.SelectAsync(id);
				else
					Document.LocalClient?.UnselectAsync(id);
			}
		}

		internal void SelectAllItemsEvent(object sender, Rhino.DocObjects.RhinoDeselectAllObjectsEventArgs e)
		{
			var settings = new Rhino.DocObjects.ObjectEnumeratorSettings()
			{
				ActiveObjects = true
			};

			foreach (var rhinoObject in e.Document.Objects.GetObjectList(settings))
			{
				if (rhinoObject.IsLocked) continue;

				if (!ChangeUtils.TryGetChangeId(rhinoObject, out Guid ChangeId)) continue;
				Document.LocalClient.UnselectAsync(ChangeId);
			}
		}

		#endregion

		public void Dispose()
		{
			// TODO : What if there are still things in the Queue?
			RhinoApp.Idle -= CallIdle;
		}

	}
}
