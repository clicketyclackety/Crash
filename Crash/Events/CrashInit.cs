using Crash.Utilities;
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
        internal static void OnInit(IEnumerable<Speck> specks)
        {
            Rhino.RhinoApp.WriteLine("Loading specks ...");
            LocalCache.Instance.BakeSpecks(specks);
        }

    }

}
