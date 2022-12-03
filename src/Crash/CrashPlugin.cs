using Crash.UI;
using Crash.Utilities;
using Rhino;
using Rhino.PlugIns;
using System;

namespace Crash
{
    ///<summary>
    /// The crash plugin for multi user rhino collaboration
    ///</summary>
    public class CrashPlugin : Rhino.PlugIns.PlugIn
    {
        public CrashPlugin()
        {
            Instance = this;
        }

        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            new InteractivePipe() { Enabled = true };
            LocalCache.Instance = new LocalCache();
            return base.OnLoad(ref errorMessage);
        }

        protected override void OnShutdown()
        {
            ServerManager.LocalServer?.Dispose();
            ServerManager.LocalServer = null;
            base.OnShutdown();
        }

        public override PlugInLoadTime LoadTime => PlugInLoadTime.AtStartup;

        protected override string LocalPlugInName => "Crash";

        ///<summary>Gets the only instance of the CrashPlugin plug-in.</summary>
        public static CrashPlugin Instance { get; private set; }

        // You can override methods here to change the plug-in behavior on
        // loading and shut down, add options pages to the Rhino _Option command
        // and maintain plug-in wide options in a document.
    }
}