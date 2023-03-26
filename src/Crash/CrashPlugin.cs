using Rhino.PlugIns;

namespace Crash
{

	///<summary>
	/// The crash plugin for multi user rhino collaboration
	///</summary>
	public sealed class CrashPlugin : CrashPluginBase
	{

		/// <inheritdoc />
		protected override string LocalPlugInName => "Crash";

		///<summary>Gets the only instance of the CrashPlugin plug-in.</summary>
		public static CrashPlugin Instance { get; private set; }

		public CrashPlugin()
		{
			Instance = this;
		}

		/// <inheritdoc />
		protected override LoadReturnCode OnLoad(ref string errorMessage)
		{
			InteractivePipe.Active = new InteractivePipe() { Enabled = false };

			LoadCrashPlugins();

			return base.OnLoad(ref errorMessage);
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

	}

}
