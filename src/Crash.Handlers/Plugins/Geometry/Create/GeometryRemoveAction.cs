using Crash.Common.Changes;
using Crash.Handlers.InternalEvents;

namespace Crash.Handlers.Plugins.Geometry.Create
{

	/// <summary>Handles Removed Objects</summary>
	internal sealed class GeometryRemoveAction : IChangeCreateAction
	{
		/// <inheritdoc/>
		public ChangeAction Action => ChangeAction.Remove;

		/// <inheritdoc/>
		public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
			=> crashArgs.Args is CrashObjectEventArgs rargs &&
			   rargs.ChangeId != Guid.Empty;

		/// <inheritdoc/>
		public bool TryConvert(object sender, CreateRecieveArgs crashArgs, out IEnumerable<IChange> changes)
		{
			changes = Array.Empty<IChange>();
			if (crashArgs.Args is not CrashObjectEventArgs rargs) return false;

			if (rargs.ChangeId == Guid.Empty) return false;

			var _user = crashArgs.Doc.Users.CurrentUser.Name;

			var removeChange = new Change(rargs.ChangeId, _user, null)
			{
				Type = GeometryChange.ChangeType,
				Action = ChangeAction.Remove,
			};

			changes = new List<IChange> { removeChange };

			return true;
		}

	}

}
