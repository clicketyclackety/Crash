using Rhino.Geometry;

namespace Crash.Events.Args
{

    internal sealed class DeleteArgs : EventArgs
    {

        internal readonly RhinoDoc Doc;
        internal readonly Guid SpeckId;


        public DeleteArgs(RhinoDoc rdoc, Guid speckId)
        {
            Doc = rdoc;
            SpeckId = speckId;
        }

    }

}
