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
            RhinoApp.WriteLine("Loading specks ...");

            IsInit = true;
            _HandleSpecks(specks);
            IsInit = false;

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
            SpeckInstance localSpeck = new SpeckInstance(speck);
            if (!speck.Temporary)
            {
                LocalCache.Instance.BakeSpeck(localSpeck);
            }
            else
            {
                if (speck.LockedBy == User.CurrentUser.Name)
                {
                    LocalCache.Instance.BakeSpeck(localSpeck);
                }
                else
                {
                    Document.CrashDoc.ActiveDoc.Users.Add(speck.Owner);
                    LocalCache.Instance.UpdateSpeck(localSpeck);
                }
            }
        }

    }

}
