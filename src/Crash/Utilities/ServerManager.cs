namespace Crash.Utilities
{
    /// <summary>
    /// The server manager
    /// </summary>
    public static class ServerManager
    {

        private static bool runningOnOSX = Rhino.Runtime.HostUtils.RunningOnOSX;

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
            string resultMessage = string.Empty;
            bool result = false;

            ShutdownLocalServer();

            CrashServer server = new CrashServer();
            LocalServer = server;

            result = server.Start(url, runningOnOSX, out resultMessage);

            Rhino.RhinoApp.WriteLine(resultMessage);

            return result;
        }

        public static void ShutdownLocalServer()
        {
            LocalServer?.Stop();
            LocalServer = null;
        }

    }

}
