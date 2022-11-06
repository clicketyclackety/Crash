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
            //TODO: if an item is removed, remove the item from the database
            RequestManager.LocalClient?.Delete(e.ObjectId);

        }

    }

}
