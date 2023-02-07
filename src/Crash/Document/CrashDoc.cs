using Crash.Events;
using Crash.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Document
{

    public sealed class CrashDoc : IDisposable
    {

        #region Document Handling

        /// <summary>
        /// The Active Crash Document.
        /// This will be updated each time the Document Changes.
        /// </summary>
        public static CrashDoc ActiveDoc { get; private set; }

        // Path will likely never exist
        // For multiple docs does it have to?
        // TODO : Swap?
        private static Dictionary<RhinoDoc, CrashDoc> activeDocuments;

        static CrashDoc()
        {
            activeDocuments = new Dictionary<RhinoDoc, CrashDoc>();
            RhinoDoc.ActiveDocumentChanged += documentChanged;
        }

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

        #endregion

        #region Tables

        public readonly UserTable Users;

        public readonly CacheTable CacheTable;

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

        private CrashDoc()
        {
            Users = new UserTable();
            CacheTable = new CacheTable();
            Cameras = new CameraTable();
        }

        public CrashDoc(RhinoDoc doc) : this()
        {
            activeDocuments.Remove(doc);
            activeDocuments.Add(doc, this);
            ActiveDoc = this;

            Queue = new IdleQueue();
        }

        internal static CrashDoc CreateHeadless()
        {
            // Register with a random string?
            return new CrashDoc();
        }

        #endregion

        #region Methods


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
