namespace Crash.Events
{
    /// <summary>
    /// Remove item event handler
    /// </summary>
    internal static class RemoveItem
    {

        internal static void Event(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
        {
            var id = LocalCache.GetSpeckId(e.TheObject);
            if (id != null)
                ClientManager.LocalClient?.Delete(id.Value);
        }

    }

}
