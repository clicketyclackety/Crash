using Crash.Common.Changes;
using Crash.Utils;

using Rhino.DocObjects;

namespace Crash.Handlers.Plugins.Geometry.Create
{
	internal sealed class GeometryUnSelectAction : IChangeCreateAction
	{
		public ChangeAction Action => ChangeAction.Unlock;

		public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
			=> crashArgs.Args is RhinoObjectSelectionEventArgs rargs &&
			   !rargs.Selected;

		public bool TryConvert(object sender, CreateRecieveArgs crashArgs, out IEnumerable<IChange> changes)
		{
			changes = Array.Empty<IChange>();
			if (crashArgs.Args is not RhinoObjectSelectionEventArgs rargs) return false;

			var userName = crashArgs.Doc.Users.CurrentUser.Name;

			changes = getChanges(rargs.RhinoObjects, crashArgs, userName);

			return true;
		}

		private IEnumerable<IChange> getChanges(IEnumerable<RhinoObject> rhinoObjects,
												CreateRecieveArgs crashArgs,
												string userName)
		{
			foreach (var rhinoObject in rhinoObjects)
			{
				if (!rhinoObject.TryGetChangeId(out Guid rhinoId)) continue;
				if (!crashArgs.Doc.CacheTable.TryGetValue(rhinoId, out GeometryChange geomChange)) continue;

				IChange change = new Change(geomChange.Id, userName, null)
				{
					Action = ChangeAction.Lock,
				};

				yield return change;
			}
		}

	}

}
