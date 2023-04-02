using Crash.Common.Changes;

namespace Crash.Handlers.Plugins.Geometry.Create
{
	internal sealed class GeometryCreateAction : IChangeCreateAction
	{
		public ChangeAction Action => ChangeAction.Add;

		public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
			=> crashArgs.Args is RhinoObjectEventArgs rargs &&
			   rargs.TheObject is not null;

		public bool TryConvert(object sender, CreateRecieveArgs crashArgs, out IEnumerable<IChange> changes)
		{
			changes = Array.Empty<IChange>();
			if (crashArgs.Args is not RhinoObjectEventArgs rargs) return false;

			var _user = crashArgs.Doc.Users.CurrentUser.Name;
			changes = new List<IChange> { GeometryChange.CreateNew(_user, rargs.TheObject.Geometry) };

			return true;
		}

	}

}
