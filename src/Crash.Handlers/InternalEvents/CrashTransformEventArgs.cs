using Crash.Geometry;

namespace Crash.Handlers.InternalEvents
{

	/// <summary>Wraps Rhino Transform Event Args</summary>
	public sealed class CrashTransformEventArgs : EventArgs
	{

		/// <summary>The CTransform of the Event</summary>
		public readonly CTransform Transform;
		/// <summary>The affected Objects</summary>
		public readonly IEnumerable<CrashObject> Objects;
		/// <summary>Will objects be copied?</summary>
		public readonly bool ObjectsWillBeCopied;

		/// <summary>Default Constructor</summary>
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
