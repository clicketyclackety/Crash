using Crash.Utilities;
using Rhino;
using Rhino.Commands;

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

            if (null == RequestManager.LocalClient) return;

            // Crash
            RequestManager.LocalClient.OnSelect += CrashSelect.OnSelect;
            RequestManager.LocalClient.OnUnselect += CrashSelect.OnUnSelect;

			RequestManager.LocalClient.OnInitialize += CrashInit.OnInit;

			RequestManager.LocalClient.OnAdd += LocalCache.OnAdd;
            RequestManager.LocalClient.OnDelete += LocalCache.OnDelete;
			RequestManager.LocalClient.OnDone += LocalCache.CollaboratorIsDone;

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

			if (null == RequestManager.LocalClient) return;

            // Crash
            RequestManager.LocalClient.OnSelect -= CrashSelect.OnSelect;
            RequestManager.LocalClient.OnUnselect -= CrashSelect.OnUnSelect;

            RequestManager.LocalClient.OnInitialize -= CrashInit.OnInit;

            RequestManager.LocalClient.OnAdd -= LocalCache.OnAdd;
            RequestManager.LocalClient.OnDelete -= LocalCache.OnDelete;
            RequestManager.LocalClient.OnDone -= LocalCache.CollaboratorIsDone;
        }

    }

}
