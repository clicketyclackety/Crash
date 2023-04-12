using Crash.Common.Changes;
using Crash.Handlers.InternalEvents;
using Crash.Utils;

namespace Crash.Handlers.Plugins.Geometry.Create
{
	internal sealed class GeometryUnSelectAction : IChangeCreateAction
	{
		public ChangeAction Action => ChangeAction.Unlock;

		public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
			=> crashArgs.Args is CrashSelectionEventArgs cargs &&
			!cargs.Selected;

		public bool TryConvert(object sender, CreateRecieveArgs crashArgs, out IEnumerable<IChange> changes)
		{
			changes = Array.Empty<IChange>();
			if (crashArgs.Args is not CrashSelectionEventArgs cargs) return false;
			string userName = crashArgs.Doc.Users.CurrentUser.Name;

			if (cargs.All)
			{
				var guids = ChangeUtils.GetSelected().ToList();
				ChangeUtils.ClearSelected();
				changes = getChanges(guids, userName);
			}
			else
			{
				changes = getChanges(cargs.CrashObjects, userName);
			}

			return true;
		}

		private IEnumerable<IChange> getChanges(IEnumerable<CrashObject> crashObjects, string userName)
		{
			foreach (var crashObject in crashObjects)
			{
				yield return CreateChange(crashObject.ChangeId, userName);
			}
		}

		private IEnumerable<IChange> getChanges(IEnumerable<Guid> changeIds, string userName)
		{
			foreach (var changeId in changeIds)
			{
				yield return CreateChange(changeId, userName);
			}
		}

		private Change CreateChange(Guid changeId, string userName)
		{
			var change = new Change(changeId, userName, null)
			{
				Action = ChangeAction.Unlock,
				Type = GeometryChange.ChangeType,
			};

			return change;
		}

	}

}
