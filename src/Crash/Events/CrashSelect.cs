using Crash.Document;

namespace Crash.Events
{
    /// <summary>
    /// Crash selection event 
    /// </summary>
    internal static class CrashSelect
    {
        /// <summary>
        /// When selected
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="speckId">the speckId</param>
        internal static void OnSelect(string name, Guid speckId)
        {
            if (null == CrashDoc.ActiveDoc?.CacheTable) return;

            // FIXME : Fix this for Mac
            var _doc = Rhino.RhinoDoc.ActiveDoc;
            Guid rObjId = CrashDoc.ActiveDoc.CacheTable.GetHost(speckId);
            if (Guid.Empty == rObjId) return;

            _doc.Objects.Lock(rObjId, true);
            _doc.Views.Redraw();
        }

        /// <summary>
        /// When unselected
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="speckId">speckId</param>
        internal static void OnUnSelect(string name, Guid speckId)
        {
            if (null == CrashDoc.ActiveDoc?.CacheTable) return;

            // FIXME : Fix this for Mac
            var _doc = Rhino.RhinoDoc.ActiveDoc;
            Guid rObjId = CrashDoc.ActiveDoc.CacheTable.GetHost(speckId);
            if (Guid.Empty == rObjId) return;

            _doc.Objects.Unlock(rObjId, true);
            _doc.Views.Redraw();
        }

    }

}
