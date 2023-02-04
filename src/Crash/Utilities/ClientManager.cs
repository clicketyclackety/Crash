using Crash.Document;
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
        internal static CrashClient? LocalClient { get; set; }

        /// <summary>
        /// Method to load the client
        /// </summary>
        /// <param name="uri">the uri of the client</param>
        public static async Task StartOrContinueLocalClient(Uri uri)
        {
            if (LocalClient is object) return;

            if (null == User.CurrentUser)
            {
                throw new System.Exception("A User has not been assigned!");
            }

            CrashClient client = new CrashClient(User.CurrentUser.Name, uri);
            LocalClient = client;
            Events.EventManagement.RegisterEvents();

            CrashDoc.ActiveDoc = new CrashDoc();

            // TODO : Check for successful connection
            await client.StartAsync();
        }

        /// <summary>
        /// Closes the local Client
        /// </summary>
        public static async Task CloseLocalClient()
        {
            Events.EventManagement.DeRegisterEvents();

            if (LocalClient is object)
                await LocalClient.StopAsync();

            LocalClient = null;
            CrashDoc.ActiveDoc?.Dispose();
        }

        /// <summary>
        /// Checks for an active Server Connection
        /// </summary>
        /// <returns>True if active, false otherwise</returns>
        public static bool CheckForActiveClient()
            => LocalClient is object;

    }

}
