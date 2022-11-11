using Rhino.Runtime;
using SpeckLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Utilities
{
    /// <summary>
    /// The server manager
    /// </summary>
    public static class ServerManager
    {
        /// <summary>
        /// local server instance
        /// </summary>
        internal static Crash.CrashServer LocalServer;

        /// <summary>
        /// Method to load the server
        /// </summary>
        /// <param name="url">the uri of the server</param>
        public static void StartOrContinueLocalServer(string url)
        {
            if (null == LocalServer)
            {
                CrashServer server = new CrashServer();
                LocalServer = server;

                server.Start(url, HostUtils.RunningOnOSX);
            }
        }
    }

}
