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
    /// <summary>
    /// Open shared model command
    /// </summary>
    public sealed class OpenSharedModel : Command
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public OpenSharedModel()
        {
            Instance = this;
        }

        /// <summary>
        /// The instance
        /// </summary>
        public static OpenSharedModel Instance { get; private set; }

        /// <summary>
        /// The command name
        /// </summary>
        public override string EnglishName => "OpenSharedModel";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var name="";
            Rhino.Input.RhinoGet.GetString("Your Name", true, ref name);
            User user = new User(name);
            User.CurrentUser = user;
            
            var URL="http://localhost:5000";
            Rhino.Input.RhinoGet.GetString("Server URL", true, ref URL);

            RequestManager.StartOrContinueLocalClient(new Uri(URL + "/Crash"));

            return Result.Success;
        }

    }

}
