using Crash.Document;
using Crash.Tables;

namespace Crash.Events
{

    /// <summary>
    /// Select item event handler
    /// </summary>
    internal static class SelectItem
    {

        internal static void Event(object sender, Rhino.DocObjects.RhinoObjectSelectionEventArgs e)
        {
            if (null == CrashDoc.ActiveDoc?.LocalClient) return;

            foreach (var rhinoObject in e.RhinoObjects)
            {
                if (rhinoObject.IsLocked)
                    continue;

                var speckId = CacheTable.GetSpeckId(rhinoObject);

                if (speckId == null)
                    continue;

                if(e.Selected)
                    CrashDoc.ActiveDoc.LocalClient?.Select(speckId.Value);
                else
                    CrashDoc.ActiveDoc.LocalClient?.Unselect(speckId.Value);

            }

        }

    }

}
