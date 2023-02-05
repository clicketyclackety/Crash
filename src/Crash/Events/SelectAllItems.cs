using Crash.Document;
using Crash.Tables;

namespace Crash.Events
{
    /// <summary>
    /// Select all items event handler
    /// </summary>
    internal static class SelectAllItems
    {
        internal static void Event(object sender, Rhino.DocObjects.RhinoDeselectAllObjectsEventArgs e)
        {
            if (null == CrashDoc.ActiveDoc?.LocalClient) return;

            var settings = new Rhino.DocObjects.ObjectEnumeratorSettings()
            {
                ActiveObjects = true
            };
            
            foreach (var rhinoObject in e.Document.Objects.GetObjectList(settings).ToList())
            {
                if (!rhinoObject.IsLocked)
                {
                    var speckId = CacheTable.GetSpeckId(rhinoObject);
                    if (null == speckId) continue;

                    CrashDoc.ActiveDoc.LocalClient.Unselect(speckId.Value);
                }

            }
        }
    }
}
