using Crash.Utils;

using Rhino.DocObjects;
using Rhino.Geometry;

namespace Crash.Handlers.InternalEvents
{

	public sealed class CrashObject
	{

		public readonly Guid ChangeId;
		public readonly Guid RhinoId;
		public readonly GeometryBase Geometry;

		public CrashObject(Guid changeId, Guid rhinoId, GeometryBase geometry)
		{
			ChangeId = changeId;
			RhinoId = rhinoId;
			Geometry = geometry;
		}

		public CrashObject(RhinoObject rhinoObject)
		{
			RhinoId = rhinoObject.Id;
			Geometry = rhinoObject.Geometry;
			rhinoObject.TryGetChangeId(out ChangeId);
		}


	}

}
