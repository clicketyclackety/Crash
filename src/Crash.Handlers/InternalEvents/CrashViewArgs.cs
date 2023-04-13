using Crash.Geometry;

using Rhino.Display;

namespace Crash.Handlers.InternalEvents
{

	/// <summary>Wraps Rhino View Event Args</summary>
	public sealed class CrashViewArgs : EventArgs
	{

		/// <summary>The Camera Location of the Event</summary>
		public readonly CPoint Location;
		/// <summary>The Camera Target of the Event</summary>
		public readonly CPoint Target;

		/// <summary>Lazy Constructor</summary>
		public CrashViewArgs(RhinoView view)
			: this(view.ActiveViewport.CameraLocation.ToCrash(),
				  view.ActiveViewport.CameraTarget.ToCrash())
		{ }


		/// <summary>Constructor mainly for Tests</summary>
		public CrashViewArgs(CPoint location, CPoint target)
		{
			Location = location;
			Target = target;
		}

	}

}
