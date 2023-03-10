using Crash.Common.Document;
using Crash.Handlers;
using Crash.Handlers.Plugins;
using Crash.Views;

using Rhino.PlugIns;
using Rhino.UI;

namespace Crash
{

	public class CrashPluginBase : PlugIn, ICrashPlugin
	{

		public sealed override PlugInLoadTime LoadTime => PlugInLoadTime.Disabled;

		protected override void OnShutdown()
		{
			foreach (CrashDoc crashDoc in CrashDocRegistry.GetOpenDocuments())
			{
				crashDoc?.LocalServer?.Stop();
				crashDoc?.LocalClient?.StopAsync().RunSynchronously();
			}
		}

		protected override LoadReturnCode OnLoad(ref string errorMessage)
		{
			// Add feature flags as advanced settings here!

			return LoadReturnCode.Success;
		}

		// Save Order in Settings
		internal static void LoadCrashPlugins()
		{
			IEnumerable<Guid> pluginIds = PlugIn.GetInstalledPlugIns().Keys;
			foreach (Guid pluginId in pluginIds)
			{
				var plugin = PlugIn.Find(pluginId);
				if (plugin is not CrashPluginBase pluginBase) continue;
				PlugIn.LoadPlugIn(pluginId);
			}
		}

		protected override void DocumentPropertiesDialogPages(RhinoDoc doc, List<OptionsDialogPage> pages)
		{
			pages.Add(new CrashPropertiesPage());
			// Create a custom OptionsDialogPage with Eto! Check samples.
			// Plugin load order should be here
		}

	}

}
