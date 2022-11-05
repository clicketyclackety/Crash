using Rhino;
using Rhino.Commands;

namespace Crash.Events
{
	internal static class EventManagement
	{

		internal static void RegisterEvents()
		{
			// Remove all events first just in case.
			DeRegisterEvents();
			RhinoDoc.AddRhinoObject += AddItem.Event;
			RhinoDoc.DeleteRhinoObject += RemoveItem.Event;
			RhinoDoc.SelectObjects += SelectItem.Event;
			RhinoDoc.DeselectObjects += SelectItem.Event;
			RhinoDoc.DeselectAllObjects += SelectAllItems.Event;
		}

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
