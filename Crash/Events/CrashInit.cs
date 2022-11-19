using Crash.Utilities;
using Eto.Forms;
using SpeckLib;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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
            Rhino.RhinoApp.WriteLine("Loading specks ...");

            IsInit = true;
            _HandleSpecks(specks);
            IsInit = false;

            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }

        private static void _HandleSpecks(IEnumerable<ISpeck> specks)
        {
            var enumer = specks.GetEnumerator();
            while(enumer.MoveNext())
            {
                _HandleSpeck(enumer.Current);
            }
        }

        private static void _HandleSpeck(ISpeck speck)
        {
            SpeckInstance localSpeck = new SpeckInstance(speck);
            if (!speck.Temporary)
            {
                LocalCache.Instance.BakeSpeck(localSpeck);
            }
            else
            {
                if (speck.LockedBy == User.CurrentUser.name)
                {
                    LocalCache.Instance.BakeSpeck(localSpeck);
                }
                else
                {
                    LocalCache.Instance.UpdateSpeck(localSpeck);
                }
            }
        }

    }

}
