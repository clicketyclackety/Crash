using Crash.Common.Changes;

namespace Crash.Handlers.Plugins.Geometry.Create
{
	internal sealed class GeometryRemoveAction : IChangeCreateAction
	{
		public ChangeAction Action => ChangeAction.Remove;

		public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
			=> crashArgs.Args is RhinoObjectEventArgs rargs &&
			   rargs.TheObject.TryGetChangeId(out var rhinoId) &&
				rhinoId != Guid.Empty;

		public bool TryConvert(object sender, CreateRecieveArgs crashArgs, out IEnumerable<IChange> changes)
		{
			changes = Array.Empty<IChange>();
			if (crashArgs.Args is not RhinoObjectEventArgs rargs) return false;

			//crashArgs.Doc.CacheTable.Get
			if (!rargs.TheObject.TryGetChangeId(out var rhinoId)) return false;
			if (!crashArgs.Doc.CacheTable.TryGetValue(rhinoId, out GeometryChange geomChange)) return false;

			var _user = crashArgs.Doc.Users.CurrentUser.Name;

			var removeChange = new Change(geomChange)
			{
				Action = ChangeAction.Remove
			};

			changes = new List<IChange> { removeChange };

			return true;
		}

	}

}
