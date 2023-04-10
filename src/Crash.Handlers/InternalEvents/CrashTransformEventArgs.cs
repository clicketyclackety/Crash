using Crash.Geometry;

namespace Crash.Handlers.InternalEvents
{

	public sealed class CrashTransformEventArgs : EventArgs
	{

		public readonly CTransform Transform;
		public readonly IEnumerable<CrashObject> Objects;
		public readonly bool ObjectsWillBeCopied;

		public CrashTransformEventArgs(CTransform transform,
										IEnumerable<CrashObject> objects,
										bool objectsWillBeCopied)
		{
			Transform = transform;
			Objects = objects;
			ObjectsWillBeCopied = objectsWillBeCopied;
		}

	}

}
