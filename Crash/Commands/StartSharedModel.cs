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

        bool _LocalClientStarted = false;
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (_LocalClientStarted)
            {
                Rhino.RhinoApp.WriteLine("Server already started!");
            }

            var name="";
            var res = Rhino.Input.RhinoGet.GetString("Your Name", true, ref name);
            User user = new User(name);
            User.CurrentUser = user;

            int port = 5000;
            Rhino.Input.RhinoGet.GetInteger("Server port", false, ref port);

            // Start Server Host
            ServerManager.StartOrContinueLocalServer($"http://0.0.0.0:{port}");

            Thread.Sleep(2000);

            try
            {
                RequestManager.StartOrContinueLocalClient(new Uri($"http://127.0.0.1:{port}/Crash"));                _LocalClientStarted = true;
            }
            catch(UriFormatException ex)
            {
                Rhino.RhinoApp.Write("Please enter a valid URI!");
            }

            return Result.Success;
        }

    }

}
