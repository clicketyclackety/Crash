using Crash.Common.Changes;
using Crash.Handlers.InternalEvents;

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
				// Get pre-selected
				// De-select all
			}
			else
			{
				changes = getChanges(cargs.CrashObjects, crashArgs, userName);
			}

			return true;
		}

		private IEnumerable<IChange> getChanges(IEnumerable<CrashObject> crashObjects,
												CreateRecieveArgs crashArgs,
												string userName)
		{
			foreach (var crashObject in crashObjects)
			{
				if (!crashArgs.Doc.CacheTable.TryGetValue(crashObject.ChangeId,
														out GeometryChange geomChange)) continue;

				IChange change = new Change(geomChange.Id, userName, null)
				{
					Action = ChangeAction.Lock,
				};

				yield return change;
			}
		}

	}

}
