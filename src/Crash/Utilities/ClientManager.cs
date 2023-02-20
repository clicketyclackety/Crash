using Crash.Client;
using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Events;
using System.Net.NetworkInformation;

namespace Crash.Utilities
{

    /// <summary>
    /// The request manager
    /// </summary>
    public sealed class ClientManager
    {
        public const string CrashPath = "Crash";
        public static string LastUrl = "http://localhost";
        public static string UrlAndPort => $"{LastUrl}{PortExt}";
        private static string PortExt => LastPort <= 0 ? string.Empty : $":{LastPort}";
        public static int LastPort = 5000;
        public static Uri ClientUri => new Uri($"{UrlAndPort}/{CrashPath}");

        private CrashDoc crashDoc;

        /// <summary>
        /// Method to load the client
        /// </summary>
        /// <param name="uri">the uri of the client</param>
        public async Task StartOrContinueLocalClient(Uri uri)
        {
            if (null == CrashDoc.ActiveDoc) return;

            string userName = CrashDoc.ActiveDoc?.Users?.CurrentUser?.Name;

            if (string.IsNullOrEmpty(userName))
            {
                throw new System.Exception("A User has not been assigned!");
            }

            CrashClient client = new CrashClient(userName, uri);
            crashDoc.LocalClient = client;

            // Crash
            client.OnSelect += CrashSelect.OnLock;
            client.OnUnselect += CrashSelect.OnUnLock;

            client.OnInitialize += Init;
            client.OnAdd += crashDoc.CacheTable.OnAdd;
            client.OnDelete += crashDoc.CacheTable.OnDelete;
            client.OnDone += crashDoc.CacheTable.CollaboratorIsDone;
            client.OnCameraChange += crashDoc.Cameras.OnCameraChange;

            // TODO : Check for successful connection
            await client.StartAsync();
        }

        /// <summary>
        /// Closes the local Client
        /// </summary>
        public async Task CloseLocalClient()
        {
            var client = crashDoc?.LocalClient;
            if (null == client) return;

            client.OnAdd -= crashDoc.CacheTable.OnAdd;
            client.OnDelete -= crashDoc.CacheTable.OnDelete;
            client.OnDone -= crashDoc.CacheTable.CollaboratorIsDone;
            client.OnCameraChange -= crashDoc.Cameras.OnCameraChange;

            await client.StopAsync();

            client = null;
            crashDoc.Dispose();
        }

        /// <summary>
        /// Checks for an active Server Connection
        /// </summary>
        /// <returns>True if active, false otherwise</returns>
        public static bool CheckForActiveClient()
            => crashDoc?.LocalClient is object;

        private void Init(IEnumerable<Common.Changes.ICachedChange> Changes)
        {
            crashDoc.LocalClient.OnInitialize -= Init;

            RhinoApp.WriteLine("Loading Changes ...");

            crashDoc.CacheTable.IsInit = true;
            try
            {
                _HandleChanges(Changes);
            }
            catch
            {

            }
            finally
            {
                crashDoc.CacheTable.IsInit = false;
            }
        }

        internal void OnLock(string name, Guid ChangeId)
        {
            if (null == crashDoc?.CacheTable) return;

            var _doc = crashDoc.HostRhinoDoc;
            Guid rObjId = crashDoc.CacheTable.GetHost(ChangeId);
            if (Guid.Empty == rObjId) return;

            _doc.Objects.Lock(rObjId, true);
            _doc.Views.Redraw();
        }

        internal void OnUnLock(string name, Guid ChangeId)
        {
            if (null == crashDoc?.CacheTable) return;

            var _doc = crashDoc.HostRhinoDoc;
            Guid rObjId = crashDoc.CacheTable.GetHost(ChangeId);
            if (Guid.Empty == rObjId) return;

            _doc.Objects.Unlock(rObjId, true);
            _doc.Views.Redraw();
        }

        private void _HandleChanges(IEnumerable<Common.Changes.ICachedChange> Changes)
        {
            var enumer = Changes.GetEnumerator();
            while (enumer.MoveNext())
            {
                try
                {
                    _HandleChange(enumer.Current);
                }
                catch { }
            }
        }

        private void _HandleChange(Common.Changes.ICachedChange Change)
        {
            if (null == Change || string.IsNullOrEmpty(Change.Owner)) return;

            // Handle Camera Changes // Admittedly very badly.
            if (Change.Payload?.Contains('{') == true)
            {
                GeometryChange localChange = new ChangeInstance(Change);
                if (!Change.Temporary)
                {
                    crashDoc?.CacheTable?.QueueChangeBake(localChange);
                }
                else
                {
                    if (string.IsNullOrEmpty(Change.LockedBy) ||
                        Change.LockedBy.ToLower() == CrashDoc.ActiveDoc.Users?.CurrentUser?.Name?.ToLower())
                    {
                        crashDoc.CacheTable?.QueueChangeBake(localChange);
                    }
                    else
                    {
                        crashDoc.Users?.Add(Change.Owner);
                        crashDoc.CacheTable?.UpdateChange(localChange);
                    }
                }
            }

        }


    }

}
