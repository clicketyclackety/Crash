using System;
using System.Collections.Generic;
using System.Linq;
using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Events;
using Crash.Utils;
using Rhino;

namespace Crash.Handlers
{

	public sealed class CrashDocHandlers : IDisposable
	{

		private CrashDoc? crashDoc;
		private RhinoDoc hostRhinoDoc;

		#region Document Handling

		/// <summary>
		/// The Active Crash Document.
		/// This will be updated each time the Document Changes.
		/// </summary>
		public static CrashDoc ActiveDoc { get; private set; }

		public static CrashDoc GetRelatedDocument(RhinoDoc doc)
			=> activeDocuments[doc];

		public RhinoDoc HostRhinoDoc => hostRhinoDoc;

		private bool IsActive => hostRhinoDoc == RhinoDoc.ActiveDoc;

		// Path will likely never exist
		// For multiple docs does it have to?
		// TODO : Swap?
		private static Dictionary<RhinoDoc, CrashDoc> activeDocuments;

		#endregion

		public CrashDocHandlers()
		{
			activeDocuments = new Dictionary<RhinoDoc, CrashDoc>();
			RhinoDoc.ActiveDocumentChanged += documentChanged;
		}


		public void RegisterEvents()
		{
			RhinoDoc.AddRhinoObject += AddItemEvent;
			RhinoDoc.DeleteRhinoObject += RemoveItemEvent;
			RhinoDoc.SelectObjects += SelectItemEvent;
			RhinoDoc.DeselectObjects += SelectItemEvent;
			RhinoDoc.DeselectAllObjects += SelectAllItemsEvent;
			RhinoDoc.UndeleteRhinoObject += AddItemEvent;
		}

		public CrashDoc CreateAndRegisterDocument(RhinoDoc doc)
		{
			crashDoc = new CrashDoc();
			RhinoApp.Idle += CallIdle;

			hostRhinoDoc = doc;

			activeDocuments.Remove(doc);
			activeDocuments.Add(doc, crashDoc);
			ActiveDoc = crashDoc;

			return crashDoc;
		}

		private void CallIdle(object sender, EventArgs e)
		{
			if (!crashDoc.Queue.TryDequeue(out IdleAction action)) return;

			action.Invoke();

			// Only runs after a queue is finished
			if (crashDoc.Queue.Count == 0)
			{
				hostRhinoDoc.Views.Redraw();
			}
		}

		// TODO : Move to Crash
		#region Events

		private static void documentChanged(object sender, DocumentEventArgs e)
		{
			if (activeDocuments.ContainsKey(e.Document))
			{
				ActiveDoc = activeDocuments[e.Document];
			}
			else
			{
				ActiveDoc = null;
			}
		}

		private void SelectItemEvent(object sender, Rhino.DocObjects.RhinoObjectSelectionEventArgs e)
		{
			if (crashDoc?.LocalClient is not object) return;

			foreach (var rhinoObject in e.RhinoObjects)
			{
				if (rhinoObject.IsLocked)
					continue;

				var ChangeId = Utils.ChangeUtils.GetChangeId(rhinoObject);

				if (ChangeId == null)
					continue;

				if (e.Selected)
					crashDoc.LocalClient?.Select(ChangeId.Value);
				else
					crashDoc.LocalClient?.Unselect(ChangeId.Value);

			}

		}

		private void AddItemEvent(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
		{
			if (null == crashDoc?.CacheTable) return;
			if (crashDoc.CacheTable.IsInit) return;
			if (crashDoc.CacheTable.SomeoneIsDone) return;

			var userName = crashDoc.Users?.CurrentUser?.Name;
			if (string.IsNullOrEmpty(userName))
			{
				Console.WriteLine("Current User is null");
				return;
			}

			var Change = GeometryChange.CreateNew(userName, e.TheObject.Geometry);
			ChangeUtils.SyncHost(e.TheObject, Change);

			var serverChange = new Change(Change);
			crashDoc?.LocalClient?.Add(serverChange);
		}

		internal void RemoveItemEvent(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
		{
			if (null == crashDoc?.LocalClient) return;

			var id = ChangeUtils.GetChangeId(e.TheObject);
			if (id != null)
				crashDoc.LocalClient.Delete(id.Value);
		}

		internal void SelectAllItemsEvent(object sender, Rhino.DocObjects.RhinoDeselectAllObjectsEventArgs e)
		{
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

					crashDoc.LocalClient.Unselect(ChangeId.Value);
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
