using Rhino.Commands;


namespace Crash.Commands
{

    public sealed class ToggleUsersUI : Command
    {

        public override string EnglishName => "ToggleCrashUI";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            UsersUIModeless.ToggleFormVisibility();
            return Result.Success;
        }
    }
}
