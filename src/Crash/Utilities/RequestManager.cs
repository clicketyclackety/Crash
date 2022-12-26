using Rhino;
using SpeckLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Utilities
{

    /// <summary>
    /// The request manager
    /// </summary>
    public static class RequestManager
    {
        /// <summary>
        /// local client instance
        /// </summary>
        internal static CrashClient LocalClient { get; set; }

        /// <summary>
        /// Method to load the client
        /// </summary>
        /// <param name="uri">the uri of the client</param>
        public static async Task StartOrContinueLocalClient(Uri uri)
        {
            if (LocalClient is object)
            {
                await ForceEndLocalClient();
            }

            if (null == LocalClient)
            {
                // TODO : Add a URI
                CrashClient client = new CrashClient(User.CurrentUser.name, uri);
                RequestManager.LocalClient = client;
                Events.EventManagement.RegisterEvents();

                await client.StartAsync();
            }

            // TODO : Check for successful connection
        }

        public static async Task ForceEndLocalClient()
        {
            Events.EventManagement.DeRegisterEvents();
            await LocalClient?.StopAsync();
            LocalClient = null;
        }

        /// <summary>
        /// Method to Push the changes and mark collabroation as done
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task CollaboratorIsDone(string name)
        {
            string sanitisedName = name.ToLower();
            IEnumerable<SpeckInstance> ToBake = LocalCache.Instance.GetSpecks().
                                        Where(s => s.Owner.ToLower() == sanitisedName);

            LocalCache.Instance.BakeSpecks(ToBake);
            LocalCache.Instance.RemoveSpecks(ToBake);
        }

    }

}
