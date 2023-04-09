using Crash.Client;
using Crash.Common.Document;
using Crash.Handlers;
using Crash.Handlers.Plugins.Camera;
using Crash.Handlers.Plugins.Geometry;
using Crash.Handlers.Plugins.Initializers;
using Crash.Handlers.Server;

using Rhino.PlugIns;

namespace Crash
{

	///<summary>
	/// The crash plugin for multi user rhino collaboration
	///</summary>
	public sealed class CrashPlugin : CrashPluginBase, IDisposable
	{
		const string _id = "53CB2393-C71F-4079-9CEC-97464FF9D14E";
		public static Guid PluginId => new(_id);

		/// <inheritdoc />
		protected override string LocalPlugInName => "Crash";

		///<summary>Gets the only instance of the CrashPlugin plug-in.</summary>
		public static CrashPlugin Instance { get; private set; }

		public CrashPlugin()
		{
			Instance = this;

			// Register the Defaults!
			RegisterChangeSchema(new GeometryChangeDefinition());
			RegisterChangeSchema(new CameraChangeDefinition());
			RegisterChangeSchema(new DoneDefinition());
		}

		/// <inheritdoc />
		protected override LoadReturnCode OnLoad(ref string errorMessage)
		{
			// Add feature flags as advanced settings here!
			InteractivePipe.Active = new InteractivePipe() { Enabled = false };
			CrashClient.OnInit += CrashClient_OnInit;

			RhinoApp.Idle += RhinoApp_Idle;

			return base.OnLoad(ref errorMessage);
		}

		private void CrashClient_OnInit(object sender, CrashClient.CrashInitArgs e)
		{
			CrashClient.OnInit -= CrashClient_OnInit;
			InteractivePipe.Active.Enabled = true;
		}

		private void RhinoApp_Idle(object sender, EventArgs e)
		{
			RhinoApp.Idle -= RhinoApp_Idle;
			if (!ServerInstaller.ServerExecutableExists)
			{
				ServerInstaller.EnsureServerExecutableExists();
			}
		}

		private void LoadCrashPlugins()
		{
			IEnumerable<Guid> pluginIds = PlugIn.GetInstalledPlugIns().Keys;
			foreach (Guid pluginId in pluginIds)
			{
				var plugin = PlugIn.Find(pluginId);
				if (plugin is not CrashPluginBase pluginBase) continue;
				PlugIn.LoadPlugIn(pluginId);
			}
		}

		/// <inheritdoc />
		protected sealed override void OnShutdown()
		{
			foreach (CrashDoc crashDoc in CrashDocRegistry.GetOpenDocuments())
			{
				crashDoc?.LocalServer?.Stop();
				crashDoc?.LocalClient?.StopAsync().RunSynchronously();
			}
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

	}

}
