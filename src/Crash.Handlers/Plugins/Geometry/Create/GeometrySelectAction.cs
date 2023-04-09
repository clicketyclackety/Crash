using Crash.Common.Changes;
using Crash.Handlers.InternalEvents;

namespace Crash.Handlers.Plugins.Geometry.Create
{
	internal sealed class GeometrySelectAction : IChangeCreateAction
	{
		public ChangeAction Action => ChangeAction.Lock;

		public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
			=> crashArgs.Args is CrashSelectionEventArgs cargs &&
			   cargs.Selected;

		public bool TryConvert(object sender, CreateRecieveArgs crashArgs, out IEnumerable<IChange> changes)
		{
			changes = Array.Empty<IChange>();
			if (crashArgs.Args is not CrashSelectionEventArgs cargs) return false;

			var userName = crashArgs.Doc.Users.CurrentUser.Name;

			changes = getChanges(cargs.CrashObjects, crashArgs, userName);

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

				IChange change = new Change(crashObject.ChangeId, userName, null)
				{
					Action = ChangeAction.Lock,
				};

				yield return change;
			}
		}

	}

}
