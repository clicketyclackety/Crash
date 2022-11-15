using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Crash
{
    /// <summary>
    /// Crash server class
    /// </summary>
    public class CrashServer : IDisposable
    {
        Process process;

        /// <summary>
        /// Empty constructor
        /// </summary>
        public CrashServer()
        {
        }

        ~CrashServer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Method to start the server
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="isMac">is mac</param>
        public bool Start(string url, bool isMac = false)
        {
            if (process != null)
                return false;

            var startInfo = new ProcessStartInfo();
            var currentPath = Path.GetDirectoryName(typeof(CrashServer).Assembly.Location);
            string serverExecutable;

            if (isMac)
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                    serverExecutable = Path.Combine(currentPath, "Server", "osx-arm64", "Crash.Server");
                else
                    serverExecutable = Path.Combine(currentPath, "Server", "osx-x64", "Crash.Server");
            }
            else
                serverExecutable = Path.Combine(currentPath, "Server", "win-x64", "Crash.Server.exe");

            if (!File.Exists(serverExecutable))
            {
                return false;
            }

            startInfo.FileName = serverExecutable;
            startInfo.Arguments = $"--urls \"{url}\"";

            process = Process.Start(startInfo);

            return true;
        }

        /// <summary>
        /// Stop connection
        /// </summary>
        public void Stop()
        {
            try
            {
                process?.Kill();
            }
            catch
            {

            }
            finally
            {
                process = null;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// Disposes of the object, stopping the server if it is running
        /// </summary>
        /// <param name="disposing">true if disposing, false if GC'd</param>
        public virtual void Dispose(bool disposing)
        {
            // stop the server!
            Stop();
        }

    }
}
