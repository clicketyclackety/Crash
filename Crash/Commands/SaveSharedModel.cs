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

    public sealed class SaveSharedModel : Command
    {

        public SaveSharedModel()
        {
            Instance = this;
        }

        public static SaveSharedModel Instance { get; private set; }

        
        public override string EnglishName => "SaveSharedModel";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Reconciliation.StartOrContinueLocalClient();

            return Result.Success;
        }

    }

}
