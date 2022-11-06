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

        public static void StartOrContinueLocalClient(string filename)
        {
            if (null == LocalClient)
            {
                // TODO : Add a URI
                Uri uri = new Uri($"<url>/{filename}");
                CrashClient client = new CrashClient(User.CurrentUser.name, uri);
                RequestManager.LocalClient = client;

                Events.EventManagement.RegisterEvents();
            }
        }

    }

}
