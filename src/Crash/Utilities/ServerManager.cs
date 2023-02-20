using Crash.Client;
using Crash.Common.Document;

namespace Crash.Utilities
{

    /// <summary>
    /// The server manager
    /// </summary>
    public sealed class ServerManager
    {
        public CrashDoc crashDoc;

        // TODO : Move these consts
        public const string DefaultURL = "http://0.0.0.0";

        public ServerManager(CrashDoc crashDoc)
        {
            this.crashDoc = crashDoc;
        }

        /// <summary>
        /// Method to load the server
        /// </summary>
        /// <param name="url">the uri of the server</param>
        public bool StartOrContinueLocalServer(string url)
        {
            CloseLocalServer();

            if (null == crashDoc) return false;
            crashDoc.LocalServer?.Stop();
            crashDoc.LocalServer = new CrashServer();

            bool result = crashDoc.LocalServer.Start(url, out string resultMessage);

            RhinoApp.WriteLine(resultMessage);

            return result;
        }

        public void CloseLocalServer()
        {
            CrashServer? server = crashDoc?.LocalServer;
            if (null == server) return;

            server?.Stop();
            crashDoc.LocalServer = null;
        }

        /// <summary>
        /// Checks for an active Server
        /// </summary>
        /// <returns>True if active, false otherwise</returns>
        public static bool CheckForActiveServer(CrashDoc activeCrashDoc)
            => activeCrashDoc?.LocalServer is object;

    }

}
