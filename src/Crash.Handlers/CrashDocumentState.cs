using System;
using System.Linq;

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
			CrashDoc? crashDoc = CrashDocRegistry.GetRelatedDocument(e.TheObject.Document);

			if (null == crashDoc?.CacheTable) return;
			if (crashDoc.CacheTable.IsInit) return;
			if (crashDoc.CacheTable.SomeoneIsDone) return;

			var userName = crashDoc.Users?.CurrentUser.Name;
			if (string.IsNullOrEmpty(userName))
			{
				Console.WriteLine("Current User is null");
				return;
			}

			var Change = GeometryChange.CreateNew(userName, e.TheObject.Geometry);
			ChangeUtils.SyncHost(e.TheObject, Change);

			var serverChange = new Change(Change);
			crashDoc?.LocalClient?.AddAsync(serverChange);
		}

		internal void RemoveItemEvent(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
		{
			CrashDoc? crashDoc = CrashDocRegistry.GetRelatedDocument(e.TheObject.Document);

			if (null == crashDoc?.LocalClient) return;

			var id = ChangeUtils.GetChangeId(e.TheObject);
			if (id != null)
				crashDoc.LocalClient.DeleteAsync(id.Value);
		}

		private void SelectItemEvent(object sender, Rhino.DocObjects.RhinoObjectSelectionEventArgs e)
		{
			CrashDoc? crashDoc = CrashDocRegistry.GetRelatedDocument(e.Document);
			if (crashDoc?.LocalClient is not object) return;

			foreach (var rhinoObject in e.RhinoObjects)
			{
				if (rhinoObject.IsLocked)
					continue;

				var ChangeId = Utils.ChangeUtils.GetChangeId(rhinoObject);

				if (ChangeId == null)
					continue;

				if (e.Selected)
					crashDoc.LocalClient?.SelectAsync(ChangeId.Value);
				else
					crashDoc.LocalClient?.UnselectAsync(ChangeId.Value);

			}

		}

		internal void SelectAllItemsEvent(object sender, Rhino.DocObjects.RhinoDeselectAllObjectsEventArgs e)
		{
			CrashDoc? crashDoc = CrashDocRegistry.GetRelatedDocument(e.Document);
			if (null == crashDoc?.LocalClient) return;

			var settings = new Rhino.DocObjects.ObjectEnumeratorSettings()
			{
				ActiveObjects = true
			};

			foreach (var rhinoObject in e.Document.Objects.GetObjectList(settings).ToList())
			{
				if (!rhinoObject.IsLocked)
				{
					var ChangeId = ChangeUtils.GetChangeId(rhinoObject);
					if (null == ChangeId) continue;

					crashDoc.LocalClient.UnselectAsync(ChangeId.Value);
				}

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
