using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using Crash.Utilities;
using System.Drawing;
using System.Security.Policy;
using System.Threading;
using static System.Net.WebRequestMethods;
using System.Xml.Linq;

namespace Crash.Commands
{
    /// <summary>
    /// Command to start the shared model
    /// </summary>
    public sealed class StartSharedModel : Command
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public StartSharedModel()
        {
            Instance = this;
        }

        /// <summary>
        /// Command Instance
        /// </summary>
        public static StartSharedModel Instance { get; private set; }

        public override string EnglishName => "StartSharedModel";
        
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (RequestManager.LocalClient is object)
            {
                RhinoApp.WriteLine("You are currently part of a Shared Model Session. " +
                    "Please restart Rhino to create or join a new session.");
            }

            string name = Environment.UserName;
            int port = 5000;

            if (!_GetUsersName(ref name))
                return Result.Nothing;

            _CreateCurrentUser(name);

            if(!_GetPortFromUser(ref port))
                return Result.Nothing;

            // TODO : Add Port Validation
            // TODO : Add Port Suggestions to docs

            // Start Server Host
            ServerManager.StartOrContinueLocalServer($"http://0.0.0.0:{port}");

            // FIXME : This is a hack for now. The Server needs to return an "I'm ready", but until then ...
            Thread.Sleep(2000);

            try
            {
                // TODO : Create these urls/ports as constants somewhere relevent
                RequestManager.StartOrContinueLocalClient(new Uri($"http://127.0.0.1:{port}/Crash"));
                RhinoApp.Write($"share url: rhino://crash&host=0.0.0.0:{port}");
            }
            catch(UriFormatException)
            {
                RhinoApp.Write("Please enter a valid host! The Port is likely bad.");
            }

            return Result.Success;
        }

        // TODO : Ensure name is not already taken!
        internal static bool _GetUsersName(ref string name)
        {
            Result getUsername = RhinoGet.GetString("Your Name", true, ref name);
            
            if (string.IsNullOrEmpty(name)) return false;
            return getUsername == Result.Success;
        }

        internal static void _CreateCurrentUser(string name)
        {
            User user = new User(name);
            User.CurrentUser = user;
        }

        internal static bool _GetPortFromUser(ref int port)
        {
            Result getPort = RhinoGet.GetInteger("Server port", false, ref port);
            if (port <= 0) return false;

            return getPort == Result.Success;
        }

    }

}
