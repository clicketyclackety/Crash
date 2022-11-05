using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using Crash.Utilities;
using System.Drawing;

namespace Crash.Commands
{

    public sealed class StartSharedModel : Command
    {

        public StartSharedModel()
        {
            Instance = this;
        }

        
        public static StartSharedModel Instance { get; private set; }

        
        public override string EnglishName => "StartSharedModel";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var name="";
            var res = Rhino.Input.RhinoGet.GetString("Your Name", true, ref name);
            User user = new User(name);
            Color color = user.color;
            return Result.Success;
        }

    }

}
