namespace Crash.Events
{
    /// <summary>
    /// Select item event handler
    /// </summary>
    internal static class SelectItem
    {
        internal static void Event(object sender, Rhino.DocObjects.RhinoObjectSelectionEventArgs e)
        {
            foreach(var rhinoObject in e.RhinoObjects)
            {
                if (rhinoObject.IsLocked)
                    continue;

                var speckId = LocalCache.GetSpeckId(rhinoObject);

                if (speckId == null)
                    continue;

                if(e.Selected)
                    ClientManager.LocalClient?.Select(speckId.Value);
                else
                    ClientManager.LocalClient?.Unselect(speckId.Value);

            }
        }

    }
}
