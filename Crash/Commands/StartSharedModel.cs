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
        /// The crash server
        /// </summary>
        CrashServer? server;

        /// <summary>
        /// Command Instance
        /// </summary>
        public static StartSharedModel Instance { get; private set; }

        public override string EnglishName => "StartSharedModel";


        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var name = "";
            Rhino.Input.RhinoGet.GetString("Your Name", true, ref name);
            User user = new User(name);
            User.CurrentUser = user;
            var URL = "http://localhost:5121";
            Rhino.Input.RhinoGet.GetString("File URL", true, ref URL);

            if (server != null)
            {
                server.Stop();
            }

            server = new CrashServer();

            server.Start(new Uri(URL));

            Thread.Sleep(2000);

            RequestManager.StartOrContinueLocalClient(new Uri(URL + "/Crash"));

            return Result.Success;
        }

    }

}
