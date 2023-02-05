using Crash.Document;
using Crash.Tables;

namespace Crash.Events
{
    /// <summary>
    /// Add item event handler
    /// </summary>
    internal static class AddItem
    {
        internal static void Event(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
        {
            if (null == CrashDoc.ActiveDoc?.CacheTable) return;

            if (CrashDoc.ActiveDoc.CacheTable.IsInit) return;
            if (CrashDoc.ActiveDoc.CacheTable.SomeoneIsDone) return;

            string? userName = CrashDoc.ActiveDoc.Users?.CurrentUser?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                Console.WriteLine("Current User is null");
                return;
            }

            SpeckInstance speck = SpeckInstance.CreateNew(userName, e.TheObject.Geometry);
            CacheTable.SyncHost(e.TheObject, speck);

            Speck serverSpeck = new Speck(speck);
            CrashDoc.ActiveDoc?.LocalClient?.Add(serverSpeck);
        }
    }
}