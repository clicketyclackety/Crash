using Crash.Document;
using Crash.Tables;

namespace Crash.Events
{
    /// <summary>
    /// Remove item event handler
    /// </summary>
    internal static class RemoveItem
    {

        internal static void Event(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
        {
            if (null == CrashDoc.ActiveDoc?.LocalClient) return;

            var id = CacheTable.GetSpeckId(e.TheObject);
            if (id != null)
                CrashDoc.ActiveDoc.LocalClient.Delete(id.Value);
        }

    }

}
