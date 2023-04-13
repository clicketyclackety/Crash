using Crash.Common.Changes;
using Crash.Handlers.InternalEvents;

namespace Crash.Handlers.Plugins.Geometry.Create
{

	/// <summary>Handles Selection</summary>
	internal sealed class GeometrySelectAction : IChangeCreateAction
	{

		/// <inheritdoc/>
		public ChangeAction Action => ChangeAction.Lock;

		/// <inheritdoc/>
		public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
		{
			if (crashArgs.Args is not CrashSelectionEventArgs cargs) return false;
			return cargs.Selected;
		}

		/// <inheritdoc/>
		public bool TryConvert(object sender, CreateRecieveArgs crashArgs, out IEnumerable<IChange> changes)
		{
			changes = Array.Empty<IChange>();
			if (crashArgs.Args is not CrashSelectionEventArgs cargs) return false;

			var userName = crashArgs.Doc.Users.CurrentUser.Name;

			changes = getChanges(cargs.CrashObjects, userName);

			return true;
		}

		private IEnumerable<IChange> getChanges(IEnumerable<CrashObject> crashObjects,
												string userName)
		{
			foreach (var crashObject in crashObjects)
			{
				if (crashObject.ChangeId == Guid.Empty) continue;

				IChange change = new Change(crashObject.ChangeId, userName, null)
				{
					Action = ChangeAction.Lock,
					Type = GeometryChange.ChangeType
				};

				yield return change;
			}
		}

	}

}
