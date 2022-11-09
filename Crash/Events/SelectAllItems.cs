using Crash.Utilities;
using Rhino.DocObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Events
{
    /// <summary>
    /// Select all items event handler
    /// </summary>
    internal static class SelectAllItems
    {
        internal static void Event(object sender, RhinoDeselectAllObjectsEventArgs e)
        {
            ObjectEnumeratorSettings settings = new ObjectEnumeratorSettings();
            settings.ActiveObjects = true;
            foreach (RhinoObject robj in e.Document.Objects.GetObjectList(settings).ToList())
            {
                if (robj.IsLocked)
                    continue;
                else
                {
                    var speckId = LocalCache.GetSpeckId(robj);
                    if (speckId != null)
                        RequestManager.LocalClient?.Unselect(speckId.Value);
                }

            }
        }
    }
}
