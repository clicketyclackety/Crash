namespace Crash.Events.Args
{

    internal sealed class DeleteArgs : EventArgs
    {

        internal readonly RhinoDoc Doc;
        internal readonly Guid ChangeId;


        public DeleteArgs(RhinoDoc rdoc, Guid changeId)
        {
            Doc = rdoc;
            ChangeId = changeId;
        }

    }

}
