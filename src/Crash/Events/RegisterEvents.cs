namespace Crash.Events
{
	/// <summary>
	/// THe rhino event manager
	/// </summary>
	internal static class EventManagement
	{
		/// <summary>
		/// Register events
		/// </summary>
		internal static void RegisterEvents()
		{
			// Remove all events first just in case.
			DeRegisterEvents();

			// Rhino
			RhinoDoc.AddRhinoObject += AddItem.Event;
			RhinoDoc.DeleteRhinoObject += RemoveItem.Event;
			RhinoDoc.SelectObjects += SelectItem.Event;
			RhinoDoc.DeselectObjects += SelectItem.Event;
			RhinoDoc.DeselectAllObjects += SelectAllItems.Event;
			RhinoDoc.UndeleteRhinoObject += AddItem.Event;

            if (null == ClientManager.LocalClient) return;

            // Crash
            ClientManager.LocalClient.OnSelect += CrashSelect.OnSelect;
            ClientManager.LocalClient.OnUnselect += CrashSelect.OnUnSelect;

			ClientManager.LocalClient.OnInitialize += CrashInit.OnInit;

			ClientManager.LocalClient.OnAdd += LocalCache.OnAdd;
            ClientManager.LocalClient.OnDelete += LocalCache.OnDelete;
			ClientManager.LocalClient.OnDone += LocalCache.CollaboratorIsDone;

        }

		/// <summary>
		/// De register events
		/// </summary>
		internal static void DeRegisterEvents()
		{
			RhinoDoc.AddRhinoObject -= AddItem.Event;
			RhinoDoc.DeleteRhinoObject -= RemoveItem.Event;
			RhinoDoc.SelectObjects -= SelectItem.Event;
			RhinoDoc.DeselectObjects -= SelectItem.Event;
			RhinoDoc.DeselectAllObjects -= SelectAllItems.Event;

			if (null == ClientManager.LocalClient) return;

            // Crash
            ClientManager.LocalClient.OnSelect -= CrashSelect.OnSelect;
            ClientManager.LocalClient.OnUnselect -= CrashSelect.OnUnSelect;

            ClientManager.LocalClient.OnInitialize -= CrashInit.OnInit;

            ClientManager.LocalClient.OnAdd -= LocalCache.OnAdd;
            ClientManager.LocalClient.OnDelete -= LocalCache.OnDelete;
            ClientManager.LocalClient.OnDone -= LocalCache.CollaboratorIsDone;
        }

    }

}
