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
    /// Select item event handler
    /// </summary>
    internal static class SelectItem
    {
        internal static void Event(object sender, RhinoObjectSelectionEventArgs e)
        {
            foreach(RhinoObject robj in e.RhinoObjects)
            {
                if (robj.IsLocked)
                    continue;
                var speckId = LocalCache.GetSpeckId(robj);
                if (speckId == null)
                    continue;

                if(e.Selected)
                    RequestManager.LocalClient?.Select(speckId.Value);
                else
                    RequestManager.LocalClient?.Unselect(speckId.Value);

            }
        }

    }
}
