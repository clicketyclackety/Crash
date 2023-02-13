using Crash.Document;
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

        private void Init(IEnumerable<ISpeck> specks)
        {
            crashDoc.LocalClient.OnInitialize -= Init;

            RhinoApp.WriteLine("Loading specks ...");

            crashDoc.CacheTable.IsInit = true;
            try
            {
                _HandleSpecks(specks);
            }
            catch
            {

            }
            finally
            {
                crashDoc.CacheTable.IsInit = false;
            }
        }

        internal void OnLock(string name, Guid speckId)
        {
            if (null == crashDoc?.CacheTable) return;

            var _doc = crashDoc.HostRhinoDoc;
            Guid rObjId = crashDoc.CacheTable.GetHost(speckId);
            if (Guid.Empty == rObjId) return;

            _doc.Objects.Lock(rObjId, true);
            _doc.Views.Redraw();
        }

        internal void OnUnLock(string name, Guid speckId)
        {
            if (null == crashDoc?.CacheTable) return;

            var _doc = crashDoc.HostRhinoDoc;
            Guid rObjId = crashDoc.CacheTable.GetHost(speckId);
            if (Guid.Empty == rObjId) return;

            _doc.Objects.Unlock(rObjId, true);
            _doc.Views.Redraw();
        }

        private void _HandleSpecks(IEnumerable<ISpeck> specks)
        {
            var enumer = specks.GetEnumerator();
            while (enumer.MoveNext())
            {
                try
                {
                    _HandleSpeck(enumer.Current);
                }
                catch { }
            }
        }

        private void _HandleSpeck(ISpeck speck)
        {
            if (null == speck || string.IsNullOrEmpty(speck.Owner)) return;

            // Handle Camera Specks // Admittedly very badly.
            if (speck.Payload?.Contains('{') == true)
            {
                SpeckInstance localSpeck = new SpeckInstance(speck);
                if (!speck.Temporary)
                {
                    crashDoc?.CacheTable?.QueueSpeckBake(localSpeck);
                }
                else
                {
                    if (string.IsNullOrEmpty(speck.LockedBy) ||
                        speck.LockedBy.ToLower() == CrashDoc.ActiveDoc.Users?.CurrentUser?.Name?.ToLower())
                    {
                        crashDoc.CacheTable?.QueueSpeckBake(localSpeck);
                    }
                    else
                    {
                        crashDoc.Users?.Add(speck.Owner);
                        crashDoc.CacheTable?.UpdateSpeck(localSpeck);
                    }
                }
            }

        }


    }

}
