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

            LocalSpeck speck = LocalSpeck.CreateNew(Guid.NewGuid(), User.CurrentUser.name, e.TheObject.Geometry);

            LocalCache.SyncHost(e.TheObject, speck);

            Speck serverSpeck = Speck.From(speck);
            RequestManager.LocalClient?.Add(serverSpeck);
        }
    }
}
