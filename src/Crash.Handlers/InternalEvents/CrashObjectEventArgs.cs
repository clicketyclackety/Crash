using Rhino.DocObjects;
using Rhino.Geometry;

namespace Crash.Handlers.InternalEvents
{

	public sealed class CrashObjectEventArgs : EventArgs
	{

		public Guid ChangeId;
		public Guid RhinoId;

		public readonly GeometryBase Geometry;

		public CrashObjectEventArgs(GeometryBase geometry, Guid rhinoId, Guid changeId = default)
		{
			ChangeId = changeId;
			RhinoId = rhinoId;
			Geometry = geometry;
		}

		public CrashObjectEventArgs(RhinoObject rhinoObject, Guid changeId = default)
		{
			ChangeId = changeId;
			RhinoId = rhinoObject.Id;
			Geometry = rhinoObject.Geometry;
		}

	}

}
