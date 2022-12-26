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

    [CommandStyle(Style.DoNotRepeat | Style.NotUndoable)]
    public sealed class OpenSharedModel : Command
    {

        public OpenSharedModel()
        {
            Instance = this;
        }

        public static OpenSharedModel Instance { get; private set; }

        /// <inheritdoc />
        public override string EnglishName => "OpenSharedModel";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (RequestManager.LocalClient is object)
            {
                RhinoApp.WriteLine("You are currently part of a Shared Model Session.");

                bool? result = _NewModelOrExit(false);
                if (!result.Value) return Result.Cancel;

                if (RhinoApp.RunScript(CloseSharedModel.Instance.EnglishName, true))
                    RhinoApp.RunScript(OpenSharedModel.Instance.EnglishName, true);

                return Result.Success;
            }

            string name = Environment.UserName;
            string url = "http://localhost:5000";

            if (!StartSharedModel._GetUsersName(ref name))
                return Result.Nothing;

            StartSharedModel._CreateCurrentUser(name);

            if (!_GetServerURL(ref url))
                return Result.Nothing;

            // TODO : Ensure Requested Server is available, and notify if not
            RequestManager.StartOrContinueLocalClient(new Uri($"{url}/Crash"));

            return Result.Success;
        }

        private bool? _NewModelOrExit(bool defaultValue)
            => CommandUtils.GetBoolean(ref defaultValue,
                "Would you like to close this model?",
                "ExitCommand",
                "CloseModel");

        private static bool _GetServerURL(ref string url)
            => CommandUtils.GetValidString("Server URL", ref url);

    }

}
