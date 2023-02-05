using Crash.Document;

namespace Crash.Events
{

    /// <summary>
    /// Crash init event handler
    /// </summary>
    internal static class CrashInit
    {

        // TODO : This needs to be done on Idle
        internal static void OnInit(IEnumerable<ISpeck> specks)
        {
            if (null == CrashDoc.ActiveDoc) return;

            RhinoApp.WriteLine("Loading specks ...");

            CrashDoc.ActiveDoc.CacheTable.IsInit = true;
            _HandleSpecks(specks);
            CrashDoc.ActiveDoc.CacheTable.IsInit = false;

            RhinoDoc.ActiveDoc.Views.Redraw();
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
            if (null == speck || string.IsNullOrEmpty(speck.Owner)) return;

            // Handle Camera Specks // Admittedly very badly.
            if (speck.Payload?.Contains("{") == true)
            {
                return;
            }

            SpeckInstance localSpeck = new SpeckInstance(speck);
            if (!speck.Temporary)
            {
                CrashDoc.ActiveDoc?.CacheTable?.BakeSpeck(localSpeck);
            }
            else
            {
                if (speck.LockedBy.ToLower() == CrashDoc.ActiveDoc.Users?.CurrentUser?.Name.ToLower())
                {
                    CrashDoc.ActiveDoc.CacheTable?.BakeSpeck(localSpeck);
                }
                else
                {
                    CrashDoc.ActiveDoc.Users.Add(speck.Owner);
                    CrashDoc.ActiveDoc.CacheTable?.UpdateSpeck(localSpeck);
                }
            }
        }

    }

}
