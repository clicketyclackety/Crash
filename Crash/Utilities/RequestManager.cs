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

        public static void StartOrContinueLocalClient(string URL)
        {
            if (null == LocalClient)
            {
                // TODO : Add a URI
                CrashClient client = new CrashClient(User.CurrentUser, null);
                RequestManager.LocalClient = client;

                Events.EventManagement.RegisterEvents();
            }
        }

    }

}
