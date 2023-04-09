using Crash.Common.Changes;
using Crash.Handlers.InternalEvents;

namespace Crash.Handlers.Plugins.Geometry.Create
{
	internal sealed class GeometryRemoveAction : IChangeCreateAction
	{
		public ChangeAction Action => ChangeAction.Remove;

		public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
			=> crashArgs.Args is CrashObjectEventArgs rargs &&
			   rargs.ChangeId != Guid.Empty;

		public bool TryConvert(object sender, CreateRecieveArgs crashArgs, out IEnumerable<IChange> changes)
		{
			changes = Array.Empty<IChange>();
			if (crashArgs.Args is not CrashObjectEventArgs rargs) return false;

			if (!crashArgs.Doc.CacheTable.TryGetValue(rargs.ChangeId,
													  out GeometryChange geomChange)) return false;

			var _user = crashArgs.Doc.Users.CurrentUser.Name;

			var removeChange = new Change(rargs.ChangeId, _user, null)
			{
				Type = GeometryChange.ChangeType,
				Action = ChangeAction.Remove
			};

			changes = new List<IChange> { removeChange };

			return true;
		}

	}

}
