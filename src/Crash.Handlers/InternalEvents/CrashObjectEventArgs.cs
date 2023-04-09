using Rhino.Geometry;

namespace Crash.Handlers.InternalEvents
{

	public sealed class CrashObjectEventArgs : EventArgs
	{

		public Guid ChangeId;

		public readonly GeometryBase Geometry;

		public CrashObjectEventArgs(GeometryBase geometry, Guid changeId = default)
		{
			Geometry = geometry;
			ChangeId = changeId;
		}

	}

}
