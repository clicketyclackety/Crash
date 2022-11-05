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
            var URL="";
            var res = Rhino.Input.RhinoGet.GetString("File URL", true, ref URL);
            RequestManager.StartOrContinueLocalClient(URL);

            return Result.Success;
        }

    }

}
