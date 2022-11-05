using Rhino.DocObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Events
{
    internal static class RemoveItem
    {
        internal static void Event(object sender, RhinoObjectEventArgs e)
        {
            //if an item is removed, remove the item from the database
        }
    }
}
