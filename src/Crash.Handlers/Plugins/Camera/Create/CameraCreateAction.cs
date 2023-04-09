using Crash.Common.Changes;

using Rhino.Display;
using Rhino.Geometry;

namespace Crash.Handlers.Plugins.Camera.Create
{
	internal sealed class CameraCreateAction : IChangeCreateAction
	{
		public ChangeAction Action => ChangeAction.Camera;

		DateTime lastSentTime;
		Point3d lastLocation;
		Point3d lastTarget;
		static TimeSpan maxPerSecond = TimeSpan.FromMilliseconds(250);

		public CameraCreateAction()
		{
			lastSentTime = DateTime.MinValue;
			lastLocation = Point3d.Unset;
			lastTarget = Point3d.Unset;
		}

		public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
		{
			if (crashArgs.Args is not ViewEventArgs viewArgs) return false;
			DateTime now = DateTime.UtcNow;
			TimeSpan timeSinceLastSent = now - lastSentTime;
			if (timeSinceLastSent < maxPerSecond)
			{
				return false;
			}

			if (viewArgs.View.ActiveViewport.CameraLocation.DistanceTo(lastLocation) < 10 &&
				viewArgs.View.ActiveViewport.CameraTarget.DistanceTo(lastTarget) < 10)
			{
				return false;
			}

			lastLocation = viewArgs.View.ActiveViewport.CameraLocation;
			lastTarget = viewArgs.View.ActiveViewport.CameraTarget;
			lastSentTime = DateTime.UtcNow;

			return true;
		}

		public bool TryConvert(object sender, CreateRecieveArgs crashArgs, out IEnumerable<IChange> changes)
		{
			changes = Array.Empty<IChange>();
			if (crashArgs.Args is not ViewEventArgs viewArgs)
			{
				changes = null;
				return false;
			}

			var userName = crashArgs.Doc.Users.CurrentUser.Name;

			var cLoc = viewArgs.View.ActiveViewport.CameraLocation;
			var cTarg = viewArgs.View.ActiveViewport.CameraTarget;

			var location = cLoc.ToCrash();
			var target = cTarg.ToCrash();
			var camera = new Common.View.Camera(location, target);

			changes = new List<IChange> { CameraChange.CreateNew(camera, userName) };

			return true;
		}

	}

}
