using Crash.Document;
using Crash.Events;
using System.Net.NetworkInformation;

namespace Crash.Utilities
{

    /// <summary>
    /// The request manager
    /// </summary>
    public static class ClientManager
    {
        public const string CrashPath = "Crash";
        public static string LastUrl = "http://localhost";
        public static string UrlAndPort => $"{LastUrl}{PortExt}";
        private static string PortExt => LastPort <= 0 ? string.Empty : $":{LastPort}";
        public static int LastPort = 5000;
        public static Uri ClientUri => new Uri($"{UrlAndPort}/{CrashPath}");

        /// <summary>
        /// local client instance
        /// </summary>
        [Obsolete("Obsolete!", true)]
        internal static CrashClient? LocalClient { get; set; }

        /// <summary>
        /// Method to load the client
        /// </summary>
        /// <param name="uri">the uri of the client</param>
        public static async Task StartOrContinueLocalClient(Uri uri)
        {
            if (null == CrashDoc.ActiveDoc) return;

            string userName = CrashDoc.ActiveDoc?.Users?.CurrentUser?.Name;

            if (string.IsNullOrEmpty(userName))
            {
                throw new System.Exception("A User has not been assigned!");
            }

            CrashClient client = new CrashClient(userName, uri);
            CrashDoc.ActiveDoc.LocalClient = client;

            // Crash
            client.OnSelect += CrashSelect.OnSelect;
            client.OnUnselect += CrashSelect.OnUnSelect;

            client.OnInitialize += CrashInit.OnInit;

            client.OnAdd += CrashDoc.ActiveDoc.CacheTable.OnAdd;
            client.OnDelete += CrashDoc.ActiveDoc.CacheTable.OnDelete;
            client.OnDone += CrashDoc.ActiveDoc.CacheTable.CollaboratorIsDone;
            client.OnCameraChange += CrashDoc.ActiveDoc.Cameras.OnCameraChange;

            // TODO : Check for successful connection
            await client.StartAsync();
        }

        /// <summary>
        /// Closes the local Client
        /// </summary>
        public static async Task CloseLocalClient()
        {
            var client = CrashDoc.ActiveDoc?.LocalClient;
            if (null == client) return;

            client.OnAdd -= CrashDoc.ActiveDoc.CacheTable.OnAdd;
            client.OnDelete -= CrashDoc.ActiveDoc.CacheTable.OnDelete;
            client.OnDone -= CrashDoc.ActiveDoc.CacheTable.CollaboratorIsDone;
            client.OnCameraChange -= CrashDoc.ActiveDoc.Cameras.OnCameraChange;

            await client.StopAsync();

            client = null;
            CrashDoc.ActiveDoc.Dispose();
        }

        /// <summary>
        /// Checks for an active Server Connection
        /// </summary>
        /// <returns>True if active, false otherwise</returns>
        public static bool CheckForActiveClient()
            => CrashDoc.ActiveDoc?.LocalClient is object;

    }

}
