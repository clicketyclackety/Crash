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
            if (CrashInit.IsInit) return;
            if (LocalCache.SomeoneIsDone) return;

            Speck speck = new Speck(e.ObjectId, User.CurrentUser.name, e.TheObject.Geometry.ToJSON(null));

            LocalCache.SyncHost(e.TheObject, speck);

            RequestManager.LocalClient?.Add(speck);
        }
    }
}
