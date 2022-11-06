using Crash.Utilities;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Drawing;
using Crash.Utilities;

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

            var name="";
            Rhino.Input.RhinoGet.GetString("Your Name", true, ref name);
            User user = new User(name);
            User.CurrentUser = user;

            // Allow the user to select a file
            var fd = new Rhino.UI.OpenFileDialog { Filter = "Rhino Files (*.3dm;)|*.3dm;" };
            if (!fd.ShowOpenDialog())
            return Rhino.Commands.Result.Cancel;

            // Verify the file that was selected
            System.Drawing.Image image;
            try
            {
                RequestManager.StartOrContinueLocalClient(fd.filename);
            }
            catch (Exception)
            {
                return Rhino.Commands.Result.Failure;
            }

            return Result.Success;
        }

    }

}
