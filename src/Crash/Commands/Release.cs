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

    /// <summary>
    /// Command to Release 
    /// </summary>
    [CommandStyle(Style.DoNotRepeat | Style.NotUndoable)]
    public sealed class ReleaseCommand : Command
    {

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
            var done = RequestManager.LocalClient.Done();
            done.RunSynchronously();

            return Result.Success;
        }

    }

}
