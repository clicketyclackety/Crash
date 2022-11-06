using SpeckLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Utilities
{

    public static class RequestManager
    {

        internal static Crash.CrashClient LocalClient;

        public static void StartOrContinueLocalClient(Uri uri)
        {
            if (null == LocalClient)
            {
                // TODO : Add a URI
                CrashClient client = new CrashClient(User.CurrentUser.name, uri);
                RequestManager.LocalClient = client;

                Events.EventManagement.RegisterEvents();

                client.StartAsync();

            }
        }

        public static async Task CollaboratorIsDone(string name)
        {
            string sanitisedName = name.ToLower();
            IEnumerable<Speck> ToBake = LocalCache.Instance.GetSpecks().
                                        Where(s => s.Owner.ToLower() == sanitisedName);

            LocalCache.Instance.BakeSpecks(ToBake);
            LocalCache.Instance.RemoveSpecks(ToBake);
        }

    }

}
