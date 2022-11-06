using Crash.Utilities;
using Rhino.DocObjects;
using SpeckLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Events
{
    /// <summary>
    /// Add item event handler
    /// </summary>
    internal static class AddItem
    {
        internal static void Event(object sender, RhinoObjectEventArgs e)
        {
            //TODO: if an item is added, add it to the database
            Speck speck = new Speck(e.ObjectId);
            RequestManager.LocalClient?.Add(speck);
        }
    }
}
