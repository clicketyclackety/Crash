using Rhino.DocObjects;
using SpeckLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Events
{
    internal static class AddItem
    {
        internal static void Event(object sender, RhinoObjectEventArgs e)
        {
            if (e.ObjectId == Guid.Empty)
                return;

            Speck speck = new Speck(e.ObjectId);
        }
    }
}
