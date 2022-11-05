using Crash.Utilities;
using Rhino.DocObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Events
{
    internal static class SelectAllItems
    {
        internal static void Event(object sender, RhinoDeselectAllObjectsEventArgs e)
        {
            foreach (RhinoObject robj in e.Document.Objects.GetSelectedObjects(false,false).ToList())
            {
                if (robj.IsLocked)
                    continue;
                else
                    RequestManager.LocalClient.Unselect(robj.Id);

            }
        }
    }
}
