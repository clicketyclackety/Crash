namespace Crash.Events
{
    /// <summary>
    /// Add item event handler
    /// </summary>
    internal static class AddItem
    {
        internal static void Event(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
        {
            if (CrashInit.IsInit) return;
            if (LocalCache.SomeoneIsDone) return;
            if (null == User.CurrentUser)
            {
                Console.WriteLine("Current User is null");
                return;
            }

            SpeckInstance speck = SpeckInstance.CreateNew(User.CurrentUser.Name, e.TheObject.Geometry);
            LocalCache.SyncHost(e.TheObject, speck);

            Speck serverSpeck = new Speck(speck);
            ClientManager.LocalClient?.Add(serverSpeck);
        }
    }
}