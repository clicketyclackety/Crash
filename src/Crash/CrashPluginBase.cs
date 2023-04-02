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
		private readonly Queue<IChangeDefinition> _changes;

		protected CrashPluginBase()
		{
			_changes = new Queue<IChangeDefinition>();
		}

		protected virtual void RegisterChangeSchema(IChangeDefinition changeDefinition)
		{
			InteractivePipe.RegisterChangeDefinition(changeDefinition);
			_changes.Enqueue(changeDefinition);

			if (Dispatcher is not null)
			{
				RegisterDefinitions();
			}
			else
			{
				CrashClient.OnConnected += CrashClient_OnConnected;
			}
		}

		private void CrashClient_OnConnected(object sender, Common.Events.CrashEventArgs e)
		{
			Dispatcher = new EventDispatcher(e.CrashDoc);
			RegisterDefinitions();
		}

		private void RegisterDefinitions()
		{
			while (_changes.Count > 0)
			{
				var changeDefinition = _changes.Dequeue();
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
