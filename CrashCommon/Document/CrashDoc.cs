using Crash.Events;
using Crash.Tables;
using Rhino.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Document
{

    public sealed class CrashDoc : IEquatable<CrashDoc>, IDisposable
    {

        private readonly Guid id;
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

        private bool IsActive => this.hostRhinoDoc == RhinoDoc.ActiveDoc;

        // Path will likely never exist
        // For multiple docs does it have to?
        // TODO : Swap?
        private static Dictionary<RhinoDoc, CrashDoc> activeDocuments;

        #endregion

        #region Tables

        public readonly UserTable Users;

        public readonly ChangeTable CacheTable;

        public readonly CameraTable Cameras;

        #endregion

        #region Connectivity

        public CrashClient? LocalClient { get; internal set; }

        public CrashServer? LocalServer { get; internal set; }

        #endregion

        #region Queue

        internal IdleQueue Queue { get; private set; }

        #endregion

        #region constructors

        static CrashDoc()
        {
            activeDocuments = new Dictionary<RhinoDoc, CrashDoc>();
            RhinoDoc.ActiveDocumentChanged += documentChanged;
        }

        private CrashDoc()
        {
            Users = new UserTable(this);
            CacheTable = new ChangeTable(this);
            Cameras = new CameraTable(this);
            Queue = new IdleQueue(this);
            id = Guid.NewGuid();

            RhinoDoc.AddRhinoObject += AddItemEvent;
            RhinoDoc.DeleteRhinoObject += RemoveItemEvent;
            RhinoDoc.SelectObjects += SelectItemEvent;
            RhinoDoc.DeselectObjects += SelectItemEvent;
            RhinoDoc.DeselectAllObjects += SelectAllItemsEvent;
            RhinoDoc.UndeleteRhinoObject += AddItemEvent;
        }

        public static CrashDoc CreateAndRegisterDocument(RhinoDoc doc)
        {
            CrashDoc crashDoc = new CrashDoc();
            crashDoc.Queue = new IdleQueue(crashDoc);
            crashDoc.hostRhinoDoc = doc;

            activeDocuments.Remove(doc);
            activeDocuments.Add(doc, crashDoc);
            ActiveDoc = crashDoc;

            return crashDoc;
        }

        internal static CrashDoc CreateHeadless()
        {
            // Register with a random string?
            return new CrashDoc();
        }

        #endregion

        #region Methods
        public bool Equals(CrashDoc other)
        {
            if (null == other) return false;
            return this.id == other.id;
        }

        public void Redraw()
        {
            if (!IsActive) return;

            RhinoDoc.ActiveDoc?.Views?.Redraw();
        }

        #endregion

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
            if (null == this?.LocalClient) return;

            foreach (var rhinoObject in e.RhinoObjects)
            {
                if (rhinoObject.IsLocked)
                    continue;

                var speckId = ChangeTable.GetSpeckId(rhinoObject);

                if (speckId == null)
                    continue;

                if (e.Selected)
                    this.LocalClient?.Select(speckId.Value);
                else
                    this.LocalClient?.Unselect(speckId.Value);

            }

        }
        private void AddItemEvent(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
        {
            if (null == this?.CacheTable) return;
            if (this.CacheTable.IsInit) return;
            if (this.CacheTable.SomeoneIsDone) return;

            string? userName = this.Users?.CurrentUser?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                Console.WriteLine("Current User is null");
                return;
            }

            SpeckInstance speck = SpeckInstance.CreateNew(userName, e.TheObject.Geometry);
            this?.CacheTable?.SyncHost(e.TheObject, speck);

            Speck serverSpeck = new Speck(speck);
            this?.LocalClient?.Add(serverSpeck);
        }

        internal void RemoveItemEvent(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
        {
            if (null == this?.LocalClient) return;

            var id = ChangeTable.GetSpeckId(e.TheObject);
            if (id != null)
                this.LocalClient.Delete(id.Value);
        }

        internal void SelectAllItemsEvent(object sender, Rhino.DocObjects.RhinoDeselectAllObjectsEventArgs e)
        {
            if (null == this?.LocalClient) return;

            var settings = new Rhino.DocObjects.ObjectEnumeratorSettings()
            {
                ActiveObjects = true
            };

            foreach (var rhinoObject in e.Document.Objects.GetObjectList(settings).ToList())
            {
                if (!rhinoObject.IsLocked)
                {
                    var speckId = ChangeTable.GetSpeckId(rhinoObject);
                    if (null == speckId) continue;

                    this.LocalClient.Unselect(speckId.Value);
                }

            }
        }

        #endregion

        // Disposal

        public void Dispose()
        {
            foreach (var key in activeDocuments.Keys)
            {
                if (activeDocuments[key] == this)
                {
                    activeDocuments.Remove(key);
                    break;
                }
            }
            
            if (ActiveDoc == this)
            {
                ActiveDoc = null;
            }

            LocalClient?.StopAsync();
            LocalServer?.Stop();

        }

    }

}
