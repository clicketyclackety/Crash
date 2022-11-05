using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Events
{

    internal static class CrashSelect
    {

        internal static void OnSelect(string name, Guid speckId)
        {
            var _doc = Rhino.RhinoDoc.ActiveDoc;
            _doc.Objects.Select(speckId, true);
        }

        internal static void OnUnSelect(string name, Guid speckId)
        {
            var _doc = Rhino.RhinoDoc.ActiveDoc;
            _doc.Objects.Select(speckId, false);
        }

    }

}
