using Crash.Tables;
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
            RhinoView.Modified += CameraTable.RhinoView_Modified;

			RhinoApp.Idle += RhinoApp_Idle;
        }

		static EventManagement()
		{
			currentQueue = new IdleQueue();
        }

		 //TODO : Move this to a real class, static may be okay, but not ideal for Mac.

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
