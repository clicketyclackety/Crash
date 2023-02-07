using Rhino.Geometry;

namespace Crash.Events.Args
{

    internal sealed class BakeArgs : EventArgs
    {

        internal readonly RhinoDoc Doc;
        internal GeometryBase? Geometry => Speck?.Geometry;
        internal readonly SpeckInstance Speck;


        public BakeArgs(RhinoDoc rdoc, SpeckInstance speck)
        {
            Doc = rdoc;
            Speck = speck;
        }

    }

}
