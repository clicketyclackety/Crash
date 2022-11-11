using Crash.Utilities;
using Eto.Forms;
using SpeckLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Events
{
    /// <summary>
    /// Crash init event handler
    /// </summary>
    internal static class CrashInit
    {
        public static bool IsInit { get; set; }

        internal static void OnInit(IEnumerable<ISpeck> specks)
        {
            IsInit = true;
            Rhino.RhinoApp.WriteLine("Loading specks ...");
            LocalCache.Instance.BakeSpecks(specks.Select(speck => LocalSpeck.ReCreate(speck)));
            IsInit = false;

            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }

    }

}
