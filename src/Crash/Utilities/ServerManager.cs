using Crash.Document;

namespace Crash.Utilities
{

    /// <summary>
    /// The server manager
    /// </summary>
    public static class ServerManager
    {

        // TODO : Move these consts
        public const string DefaultURL = "http://0.0.0.0";

        [Obsolete]
        public static int LastPort = 5000;

        /// <summary>
        /// Method to load the server
        /// </summary>
        /// <param name="url">the uri of the server</param>
        public static bool StartOrContinueLocalServer(string url)
        {
            CloseLocalServer();

            if (null == CrashDoc.ActiveDoc) return false;
            CrashServer? server = CrashDoc.ActiveDoc.LocalServer;
            if (null == server) return false;

            bool result = server.Start(url, Rhino.Runtime.HostUtils.RunningOnOSX, out string resultMessage);

            Rhino.RhinoApp.WriteLine(resultMessage);

            return result;
        }

        public static void CloseLocalServer()
        {
            CrashServer? server = CrashDoc.ActiveDoc?.LocalServer;
            if (null == server) return;

            server?.Stop();
            CrashDoc.ActiveDoc.LocalServer = null;
        }

        /// <summary>
        /// Checks for an active Server
        /// </summary>
        /// <returns>True if active, false otherwise</returns>
        public static bool CheckForActiveServer()
            => CrashDoc.ActiveDoc?.LocalServer is object;

    }

}
