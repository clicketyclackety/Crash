using Crash.Common.Document;
using Rhino.Commands;


namespace Crash.Commands
{

    /// <summary>
    /// Command to Release Changes
    /// </summary>
    [CommandStyle(Style.DoNotRepeat | Style.NotUndoable)]
    public sealed class ReleaseCommand : Command
    {

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ReleaseCommand()
        {
            Instance = this;
        }

        /// <inheritdoc />
        public static ReleaseCommand Instance { get; private set; }

        /// <inheritdoc />
        public override string EnglishName => "Release";

        /// <inheritdoc />
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO : Wait for response for data integrity check
            CrashDoc.ActiveDoc?.LocalClient?.Done();

            return Result.Success;
        }

    }

}
