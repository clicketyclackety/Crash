using Crash.Geometry;

using Rhino.Display;

namespace Crash.Handlers.InternalEvents
{

	public sealed class CrashViewArgs : EventArgs
	{

		public readonly CPoint Location;
		public readonly CPoint Target;

		public CrashViewArgs(RhinoView view)
		{
			Location = view.ActiveViewport.CameraLocation.ToCrash();
			Target = view.ActiveViewport.CameraTarget.ToCrash();
		}

		public CrashViewArgs(CPoint location, CPoint target)
		{
			Location = location;
			Target = target;
		}

	}

}
