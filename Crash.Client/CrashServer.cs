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
    public class CrashServer : IDisposable
    {
        Process process;
        public CrashServer()
        {
        }

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

        public void Stop()
        {
            process?.Kill();
            process = null;
        }


        public void Dispose()
        {
            // stop the server!
            Stop();
        }

    }
}
