namespace Crash.Utilities
{

    /// <summary>
    /// The server manager
    /// </summary>
    public static class ServerManager
    {

        public const string DefaultURL = "http://0.0.0.0";
        public static int LastPort = 5000;

        /// <summary>
        /// local server instance
        /// </summary>
        internal static CrashServer? LocalServer;

        /// <summary>
        /// Method to load the server
        /// </summary>
        /// <param name="url">the uri of the server</param>
        public static bool StartOrContinueLocalServer(string url)
        {
            CloseLocalServer();

            CrashServer server = new CrashServer();
            LocalServer = server;

            bool result = server.Start(url, Rhino.Runtime.HostUtils.RunningOnOSX, out string resultMessage);

            Rhino.RhinoApp.WriteLine(resultMessage);

            return result;
        }

        public static void CloseLocalServer()
        {
            LocalServer?.Stop();
            LocalServer = null;
        }

        /// <summary>
        /// Checks for an active Server
        /// </summary>
        /// <returns>True if active, false otherwise</returns>
        public static bool CheckForActiveServer()
            => LocalServer is object;

    }

}
