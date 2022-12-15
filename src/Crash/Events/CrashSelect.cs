using Crash.Utilities;
using Rhino.DocObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var _doc = Rhino.RhinoDoc.ActiveDoc;
            Guid rObjId = LocalCache.GetHost(speckId);
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
            var _doc = Rhino.RhinoDoc.ActiveDoc;
            Guid rObjId = LocalCache.GetHost(speckId);
            if (Guid.Empty == rObjId) return;

            _doc.Objects.Unlock(rObjId, true);
            _doc.Views.Redraw();
        }

    }

}
