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
    /// Remove item event handler
    /// </summary>
    internal static class RemoveItem
    {

        internal static void Event(object sender, RhinoObjectEventArgs e)
        {
            var id = LocalCache.GetSpeckId(e.TheObject);
            if (id != null)
                RequestManager.LocalClient?.Delete(id.Value);
        }

    }

}
