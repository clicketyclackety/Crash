using Crash.Utilities;

using Rhino;
using Rhino.Input;
using Rhino.Commands;

namespace Crash.Commands
{
    
    public sealed class CloseSharedModel : Command
    {

        public CloseSharedModel()
        {
            Instance = this;
        }

        public static CloseSharedModel Instance { get; private set; }

        public override string EnglishName => "CloseSharedModel";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            bool choice = true;
            if (!_GetReleaseChoice(ref choice)) return Result.Cancel;

            if (choice)
            {
                var task = RequestManager.LocalClient.Done();
                if (!task.Wait(500))
                {
                    RhinoApp.WriteLine("Could not Connect to server, data will not be saved.");
                }
                else
                {
                    RhinoApp.WriteLine("Model closed and saved successfully");
                }
            }

            RequestManager.ForceEndLocalClient();

            return Result.Success;
        }

        private static bool _GetReleaseChoice(ref bool choice)
        {
            Result getUrl = RhinoGet.GetBool("Do you want to release URL", true,
                                             "No Release", "Release", ref choice);

            return getUrl == Result.Success;
        }

    }

}
