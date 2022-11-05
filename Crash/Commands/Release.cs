using Crash.Utilities;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;

namespace Crash.Commands
{

    public sealed class ReleaseCommand : Command
    {

        public ReleaseCommand()
        {
            Instance = this;
        }

        public static ReleaseCommand Instance { get; private set; }

        
        public override string EnglishName => "Release";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO : Wait for response for data integrity check
            RequestManager.LocalClient.Done();

            return Result.Success;
        }

    }

}
