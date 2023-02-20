using Crash.Common.Tables;
using Crash.Document;
using Rhino.Display;


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
		internal static void RegisterStaticEvents()
		{
			// Remove all events first just in case.
			DeRegisterStaticEvents();

			// Rhino
            RhinoView.Modified += CameraTable.RhinoView_Modified;
        }

		static RhinoEventManagement()
		{

        }

        /// <summary>
        /// De register events
        /// </summary>
        internal static void DeRegisterStaticEvents()
		{
			RhinoView.Modified -= CameraTable.RhinoView_Modified;
        }

        internal static void RegisterDocumentEvents(CrashDoc crashDoc)
        {

        }

    }

}
