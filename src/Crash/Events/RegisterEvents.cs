using Crash.Tables;
using Rhino.Display;
using Rhino.Geometry;

namespace Crash.Events
{

    /// <summary>
    /// THe rhino event manager
    /// </summary>
    internal static class RhinoEventManagement
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
        }

		static RhinoEventManagement()
		{

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

}
