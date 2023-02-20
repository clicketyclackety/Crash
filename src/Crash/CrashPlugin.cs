using Crash.Common.Document;
using Crash.Handlers;

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

			return base.OnLoad(ref errorMessage);
		}

		/// <inheritdoc />
		protected override void OnShutdown()
		{
			foreach (CrashDoc crashDoc in CrashDocRegistry.GetOpenDocuments())
			{
				crashDoc?.LocalServer?.Stop();
				crashDoc?.LocalClient?.StopAsync().RunSynchronously();
			}
		}

		/// <inheritdoc />
		public override PlugInLoadTime LoadTime => PlugInLoadTime.AtStartup;

		/// <inheritdoc />
		protected override string LocalPlugInName => "Crash";

		///<summary>Gets the only instance of the CrashPlugin plug-in.</summary>
		public static CrashPlugin Instance { get; private set; }

	}
}
