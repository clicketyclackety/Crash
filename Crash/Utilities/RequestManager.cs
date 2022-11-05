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

        public static void StartOrContinueLocalClient()
        {
            if (null == LocalClient)
            {
                Events.EventManagement.RegisterEvents();
                // TODO : Add a URI
                CrashClient client = new CrashClient(User.CurrentUser, null);
                RequestManager.LocalClient = client;
            }
        }

    }

}
