using Crash.Utilities;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Crash.Commands
{

    public sealed class OpenSharedModel : Command
    {

        public OpenSharedModel()
        {
            Instance = this;
        }

        CrashServer server;


        public static OpenSharedModel Instance { get; private set; }

        
        public override string EnglishName => "OpenSharedModel";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {

            var name="";
            Rhino.Input.RhinoGet.GetString("Your Name", true, ref name);
            User user = new User(name);
            User.CurrentUser = user;
            var URL="http://localhost:5121";
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
