using Crash.Common.Changes;

using Rhino.Display;

namespace Crash.Handlers.Plugins.Camera.Create
{
	internal sealed class CameraCreateAction : IChangeCreateAction
	{
		public ChangeAction Action => ChangeAction.Camera;

		public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
			=> crashArgs.Args is ViewEventArgs;

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
