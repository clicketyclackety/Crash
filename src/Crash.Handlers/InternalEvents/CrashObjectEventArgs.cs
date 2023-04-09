using Rhino;
using Rhino.Geometry;

namespace Crash.Handlers.InternalEvents
{

	public sealed class CrashObjectEventArgs : EventArgs
	{
		public readonly GeometryBase Geometry;

		public readonly RhinoDoc Document;

		public CrashObjectEventArgs(RhinoDoc doc, GeometryBase geometry)
		{
			Geometry = geometry;
			Document = doc;
		}

	}

}
