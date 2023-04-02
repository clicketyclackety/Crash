using Crash.Handlers.Plugins;

using Rhino.PlugIns;


namespace Crash
{

	/// <summary>
	/// All CrashPlugins should inherit from this base
	/// </summary>
	public abstract class CrashPluginBase : PlugIn
	{

		protected virtual void RegisterChangeSchema(IChangeDefinition changeDefinition)
		{
			InteractivePipe.RegisterChangeDefinition(changeDefinition);
			// changeDefinition.CreateActions
			// changeDefinition.RecieveActions
			foreach (var act in changeDefinition.RecieveActions)
			{
				// act.OnRecieve(doc, change)
			}
		}

		#region Rhino Plugin Overrides 

		/// <inheritdoc />
		public sealed override PlugInLoadTime LoadTime
			=> this.GetType() == typeof(CrashPlugin) ?
				PlugInLoadTime.AtStartup :
				PlugInLoadTime.WhenNeeded;

		#endregion

	}

}
