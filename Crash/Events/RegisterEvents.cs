using Rhino;
using Rhino.Commands;

namespace Crash.Events
{
	internal static class EventManagement
	{

		internal static void RegisterEvents()
		{
			DeRegisterEvents(); // Remove all events first just in case.
								// TODO: [Structure] all registered Event are static functions, maybe it is fine now but it could bring challenges once we try to do something different according to the Branch data
			RhinoDoc.AddRhinoObject += AddItem.Event;
			RhinoDoc.DeleteRhinoObject += RemoveItem.Event;
			RhinoDoc.SelectObjects += SelectItem.Event;
			RhinoDoc.DeselectObjects += SelectItem.Event;


		}

		internal static void DeRegisterEvents()
		{
			RhinoDoc.AddRhinoObject -= AddItem.Event;
			RhinoDoc.DeleteRhinoObject -= RemoveItem.Event;
			RhinoDoc.SelectObjects -= SelectItem.Event;
			RhinoDoc.DeselectObjects -= SelectItem.Event;

		}

	}
}
