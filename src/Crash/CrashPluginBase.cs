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
			CrashClient.OnInit += CrashClient_OnInit;
		}

		private void CrashClient_OnInit(object sender, CrashClient.CrashInitArgs e)
		{
			RhinoApp.WriteLine("Loading Changes ...");

			if (Dispatcher is null)
			{
				Dispatcher = new EventDispatcher(e.CrashDoc);
			}

			if (Dispatcher is not null)
			{
				RegisterDefinitions();

				e.CrashDoc.CacheTable.IsInit = true;

				foreach (Change change in e.Changes)
				{
					Dispatcher.NotifyDispatcher(change);
				}

				e.CrashDoc.CacheTable.IsInit = false;
			}
		}

		protected virtual void RegisterChangeSchema(IChangeDefinition changeDefinition)
		{
			InteractivePipe.RegisterChangeDefinition(changeDefinition);
			_changes.Push(changeDefinition);
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
