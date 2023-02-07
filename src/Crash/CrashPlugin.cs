using Rhino.PlugIns;


namespace Crash
{

    ///<summary>
    /// The crash plugin for multi user rhino collaboration
    ///</summary>
    public sealed class CrashPlugin : PlugIn
    {

        public CrashPlugin()
        {
            Instance = this;
        }

        /// <inheritdoc />
        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            InteractivePipe.Active = new InteractivePipe() { Enabled = true };

            Events.RhinoEventManagement.RegisterEvents();

            return base.OnLoad(ref errorMessage);
        }

        /// <inheritdoc />
        protected override void OnShutdown()
        {
            ServerManager.CloseLocalServer();
            ClientManager.CloseLocalClient();
        }

        /// <inheritdoc />
        public override PlugInLoadTime LoadTime => PlugInLoadTime.AtStartup;

        /// <inheritdoc />
        protected override string LocalPlugInName => "Crash";

        ///<summary>Gets the only instance of the CrashPlugin plug-in.</summary>
        public static CrashPlugin Instance { get; private set; }

    }
}