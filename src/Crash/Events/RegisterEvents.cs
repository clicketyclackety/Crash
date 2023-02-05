using Rhino.Display;
using Rhino.Geometry;

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
            RhinoView.Modified += CameraCache.RhinoView_Modified;

            RhinoApp.Idle += RhinoApp_Idle;

            if (null == ClientManager.LocalClient) return;

            // Crash
            ClientManager.LocalClient.OnSelect += CrashSelect.OnSelect;
            ClientManager.LocalClient.OnUnselect += CrashSelect.OnUnSelect;

			ClientManager.LocalClient.OnInitialize += CrashInit.OnInit;

			ClientManager.LocalClient.OnAdd += LocalCache.OnAdd;
            ClientManager.LocalClient.OnDelete += LocalCache.OnDelete;
			ClientManager.LocalClient.OnDone += LocalCache.CollaboratorIsDone;
			ClientManager.LocalClient.OnCameraChange += CameraCache.OnCameraChange;

        }

		static EventManagement()
		{
			currentQueue = new IdleQueue();
        }

		internal static IdleQueue currentQueue;

		static int i = 0;
        private static void RhinoApp_Idle(object sender, EventArgs e)
        {
			currentQueue.RunQueue();
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

	internal sealed class IdleQueue
	{
		private ConcurrentQueue<Action> actions;

		internal IdleQueue()
		{
			actions = new ConcurrentQueue<Action>();
		}

		internal bool RunQueue()
		{
			if (!actions.TryDequeue(out Action action)) return false;

			action.Invoke();

			return actions.Count == 0;
		}

		internal void AddAction(Action action)
		{
			actions.Enqueue(action);
		}

	}

}
