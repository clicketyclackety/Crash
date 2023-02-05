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

        private static Dictionary<string, CrashDoc> activeDocuments;

        static CrashDoc()
        {
            activeDocuments = new Dictionary<string, CrashDoc>();
            RhinoDoc.ActiveDocumentChanged += documentChanged;
        }

        private static void documentChanged(object sender, DocumentEventArgs e)
        {
            string path = e.Document.Path;
            if (string.IsNullOrEmpty(path)) return;
            if (activeDocuments.ContainsKey(path))
            {
                ActiveDoc = activeDocuments[path];
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

        #endregion

        #region Connectivity

        public CrashClient? LocalClient { get; internal set; }

        public CrashServer? LocalServer { get; internal set; }

        #endregion

        #region constructors

        private CrashDoc()
        {
            Users = new UserTable();
            CacheTable = new CacheTable();
        }

        public CrashDoc(RhinoDoc doc) : this()
        {
            activeDocuments.Add(doc.Path, this);
            ActiveDoc = this;
        }

        internal static CrashDoc CreateHeadless()
        {
            // Register with a random string?
            return new CrashDoc();
        }

        #endregion

        #region Methods

        internal static CrashDoc? GetDocument(string path)
        {
            if (activeDocuments.TryGetValue(path, out CrashDoc doc))
            {
                return doc;
            }

            return null;
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
        }

    }

}
