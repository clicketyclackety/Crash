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
            ObjectEnumeratorSettings settings = new ObjectEnumeratorSettings();
            settings.ActiveObjects = true;
            foreach (RhinoObject robj in e.Document.Objects.GetObjectList(settings).ToList())
            {
                if (robj.IsLocked)
                    continue;
                else
                    RequestManager.LocalClient?.Unselect(robj.Id);

            }
        }
    }
}
