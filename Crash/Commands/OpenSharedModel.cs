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

        public static OpenSharedModel Instance { get; private set; }

        public override string EnglishName => "OpenSharedModel";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            string name = Environment.UserName;
            string url = "http://localhost:5000";

            if (!StartSharedModel._GetUsersName(ref name))
                return Result.Nothing;

            StartSharedModel._CreateCurrentUser(name);

            if (!_GetServerURL(ref url))
                return Result.Nothing;

            RequestManager.StartOrContinueLocalClient(new Uri($"{url}/Crash"));

            return Result.Success;
        }

        private static bool _GetServerURL(ref string url)
        {
            Result getUrl = RhinoGet.GetString("Server URL", true, ref url);
            
            if (string.IsNullOrEmpty(url)) return false;

            return getUrl == Result.Success;
        }

    }

}
