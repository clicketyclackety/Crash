using Crash.Client;
using Crash.Handlers.Plugins;

using Rhino.PlugIns;


namespace Crash
{

	/// <summary>
	/// All CrashPlugins should inherit from this base
	/// </summary>
	public abstract class CrashPluginBase : PlugIn
	{
		private EventDispatcher Dispatcher;
		private readonly Stack<IChangeDefinition> _changes;

		protected CrashPluginBase()
		{
			_changes = new Stack<IChangeDefinition>();
			CrashClient.OnConnected += CrashClient_OnConnected;
		}

		protected virtual void RegisterChangeSchema(IChangeDefinition changeDefinition)
		{
			InteractivePipe.RegisterChangeDefinition(changeDefinition);
			_changes.Push(changeDefinition);
		}

		private void CrashClient_OnConnected(object sender, Common.Events.CrashEventArgs e)
		{
			RhinoApp.WriteLine("Loading Changes ...");

			if (Dispatcher is null)
			{
				Dispatcher = new EventDispatcher(e.CrashDoc);
			}

			if (Dispatcher is not null)
			{
				RegisterDefinitions();
			}
		}

		private void RegisterDefinitions()
		{
			while (_changes.Count > 0)
			{
				var changeDefinition = _changes.Pop();
				Dispatcher.RegisterDefinition(changeDefinition);
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
