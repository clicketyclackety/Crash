using SpeckLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Utilities
{

    public static class Reconciliation
    {

        internal static Crash.CrashClient LocalClient;

        public static void StartOrContinueLocalClient()
        {
            if (null == LocalClient)
            {
                // TODO : Add a URI
                CrashClient client = new CrashClient(User.CurrentUser, null);
                Reconciliation.LocalClient = client;
            }
        }

        public static void Update()
        {

        }

        public static async Task ImDone()
        {
            // Send your username to the server
        }

        public static async Task CollaboratorIsDone(string name)
        {
            string sanitisedName = name.ToLower();
            IEnumerable<Speck> ToBake = LocalCache.Instance.GetSpecks().
                                        Where(s => s.Owner.ToLower() == sanitisedName);

            LocalCache.Instance.BakeSpecks(ToBake);
            LocalCache.Instance.RemoveSpecks(ToBake);
        }

        private static bool UpdateCD()
        {
            return true;
        }

        private static bool BakeSpeck()
        {
            return true;
        }

        private static bool IsMe()
        {
            return true;
        }

        private static bool IsSelected()
        {
            return true;

        }

        private static bool PerformAction()
        {
            return true;

        }

    }

}
