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

        /// <summary>
        /// Method to start the server
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="isMac">is mac</param>
        /// <param name="isArm64">is arm64</param>
        public void Start(Uri url, bool isMac = false, bool isArm64 = false)
        {
            if (process != null)
                return;

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

            startInfo.FileName = serverExecutable;
            startInfo.Arguments = $"--urls {url.AbsoluteUri}";

            process = Process.Start(startInfo);
        }

        /// <summary>
        /// Stop connection
        /// </summary>
        public void Stop()
        {
            process?.Kill();
            process = null;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            // stop the server!
            Stop();
        }

    }
}
