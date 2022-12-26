using Crash.Utilities;

using Rhino;
using Rhino.Input;
using Rhino.Commands;
using System.Threading.Tasks;
using Crash.UI;
using Rhino.Input.Custom;
using Crash.Events;

namespace Crash.Commands
{

    [CommandStyle(Style.DoNotRepeat | Style.NotUndoable)]
    public sealed class CloseSharedModel : Command
    {
        private bool defaultValue = false;

        public CloseSharedModel()
        {
            Instance = this;
        }

        public static CloseSharedModel Instance { get; private set; }

        /// <inheritdoc />
        public override string EnglishName => "CloseSharedModel";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            bool? choice = _GetReleaseChoice();
            if (null == choice) return Result.Cancel;

            EventManagement.DeRegisterEvents();

            if (choice.Value)
            {
                RequestManager.LocalClient?.Done();
            }

            RequestManager.ForceEndLocalClient();
            ServerManager.ShutdownLocalServer();

            LocalCache.Clear();
            InteractivePipe.Instance.Enabled = false;
            _EmptyModel(doc);

            RhinoApp.WriteLine("Model closed and saved successfully");

            doc.Views.Redraw();

            return Result.Success;
        }

        private bool? _GetReleaseChoice()
            => CommandUtils.GetBoolean(ref defaultValue,
                "Would you like to Release before exiting?",
                "JustExit",
                "ReleaseThenExit");

        private void _EmptyModel(Rhino.RhinoDoc doc)
        {
            doc.Objects.Clear();
        }

    }

}
