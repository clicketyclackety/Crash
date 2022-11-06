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
        /// Command Instance
        /// </summary>
        public static StartSharedModel Instance { get; private set; }

        public override string EnglishName => "StartSharedModel";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var name="";
            var res = Rhino.Input.RhinoGet.GetString("Your Name", true, ref name);
            User user = new User(name);
            User.CurrentUser = user;

            // Start Server Host

            return Result.Success;
        }

    }

}
