using Crash.Common.Changes;
using Rhino.Geometry;

namespace Crash.Events.Args
{

    internal sealed class BakeArgs : EventArgs
    {

        internal readonly RhinoDoc Doc;
        internal GeometryBase? Geometry => Change?.Geometry;
        internal readonly GeometryChange Change;


        public BakeArgs(RhinoDoc rdoc, GeometryChange Change)
        {
            Doc = rdoc;
            Change = Change;
        }

    }

}
