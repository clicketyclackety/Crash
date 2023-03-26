using Crash.Common.Document;
using Crash.Handlers;
using Crash.Handlers.Plugins.Schemas;
using Crash.Views;

using Rhino.PlugIns;
using Rhino.UI;


namespace Crash
{

	/// <summary>
	/// All CrashPlugins should inherit from this base
	/// </summary>
	public abstract class CrashPluginBase : PlugIn, IDisposable
	{

		// Register schemas in reverse order
		// Last in First Out type order
		// The events fire in reverse order
		private Dictionary<int, List<ChangeConverter<IChange>>> _changeConverters;

		protected CrashPluginBase()
		{
			_changeConverters = new Dictionary<int, List<ChangeConverter<IChange>>>();

			// Register the Defaults!
			RegisterChangeSchema(new GeometryChangeSchema());
			RegisterChangeSchema(new CameraChangeSchema());
		}

		protected void RegisterChangeSchema<TChange>(ChangeConverter<TChange> converter) where TChange : IChange
		{
			_changeConverters.Add(converter as ChangeConverter<IChange>);
			InteractivePipe.RegisterDrawFunction(converter as ChangeConverter<IChange>);
		}

		private void ChangeEventMatrix(IChange? change)
			=> ChangeEventMatrix(change, null, null);

		private void ChangeEventMatrix(Guid? guid, string? value)
		{

		}

		private void ChangeEventMatrix(IChange? change, string? value)
		{
			// Action happened
			// Which change should be created as a result of this
			// And which type of Action was it?
		}



		private void RegisterActions(CrashDoc crashDoc)
		{
			foreach (int action in actionSwitch.Keys)
			{
				var actionFunc = actionSwitch[action];
				switch (action)
				{
					case (int)ChangeAction.Add:
						crashDoc.LocalClient.OnAdd += (name, change) => ChangeEventMatrix(change, name);
						break;

					case (int)ChangeAction.Remove:
						crashDoc.LocalClient.OnDelete += (name, id) => actionFunc.Invoke(crashDoc, Change.CreateEmpty(id));
						break;

					case (int)ChangeAction.Lock:
						crashDoc.LocalClient.OnSelect += (name, id) => actionFunc.Invoke(crashDoc, Change.CreateEmpty(id));
						break;

					case (int)ChangeAction.Unlock:
						crashDoc.LocalClient.OnUnselect += (name, id) => actionFunc.Invoke(crashDoc, Change.CreateEmpty(id));
						break;

					case (int)ChangeAction.Camera:
						crashDoc.LocalClient.OnCameraChange += (name, change) => actionFunc.Invoke(crashDoc, change);
						break;

					default:

						// TODO : remove Id from this
						crashDoc.LocalClient.OnUpdate += (name, id, change) => actionFunc.Invoke(crashDoc, change);
						break;
				}
			}
		}

		#region Rhino Plugin Overrides 

		/// <inheritdoc />
		public sealed override PlugInLoadTime LoadTime
			=> this.GetType() == typeof(CrashPlugin) ?
				PlugInLoadTime.AtStartup :
				PlugInLoadTime.WhenNeeded;

		/// <inheritdoc />
		protected override LoadReturnCode OnLoad(ref string errorMessage)
		{
			// Add feature flags as advanced settings here!

			return LoadReturnCode.Success;
		}

		/// <inheritdoc />
		protected override void DocumentPropertiesDialogPages(RhinoDoc doc, List<OptionsDialogPage> pages)
		{
			pages.Add(new CrashPropertiesPage());
			// Create a custom OptionsDialogPage with Eto! Check samples.
			// Plugin load order should be here
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
			OnShutdown();
		}

		#endregion

	}

}
