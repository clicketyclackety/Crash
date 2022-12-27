namespace Crash.Events
{
    /// <summary>
    /// Select all items event handler
    /// </summary>
    internal static class SelectAllItems
    {
        internal static void Event(object sender, Rhino.DocObjects.RhinoDeselectAllObjectsEventArgs e)
        {
            var settings = new Rhino.DocObjects.ObjectEnumeratorSettings()
            {
                ActiveObjects = true
            };
            
            foreach (var rhinoObject in e.Document.Objects.GetObjectList(settings).ToList())
            {
                if (!rhinoObject.IsLocked)
                {
                    var speckId = LocalCache.GetSpeckId(rhinoObject);
                    if (null == speckId) continue;

                    ClientManager.LocalClient?.Unselect(speckId.Value);
                }

            }
        }
    }
}
