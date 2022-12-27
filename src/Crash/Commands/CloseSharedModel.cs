using Rhino.Commands;
using Rhino;

using Crash.Events;


namespace Crash.Commands
{

    /// <summary>
    /// Command to Close a Shared Model
    /// </summary>
    [CommandStyle(Style.DoNotRepeat | Style.NotUndoable)]
    public sealed class CloseSharedModel : Command
    {
        private bool defaultValue = false;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CloseSharedModel()
        {
            Instance = this;
        }

        /// <inheritdoc />
        public static CloseSharedModel Instance { get; private set; }

        /// <inheritdoc />
        public override string EnglishName => "CloseSharedModel";

        /// <inheritdoc />
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            bool? choice = _GetReleaseChoice();
            if (null == choice)
                return Result.Cancel;

            if (choice.Value)
                ClientManager.LocalClient?.Done();

            EventManagement.DeRegisterEvents();

            ServerManager.CloseLocalServer(); // TODO : Should this be closed?
            ClientManager.CloseLocalClient();

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
