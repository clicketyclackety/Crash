using Crash.Common.Changes;
using Crash.Geometry;
using Crash.Utils;

using Rhino.DocObjects;

namespace Crash.Handlers.Plugins.Geometry.Create
{
	internal sealed class GeometryTransformAction : IChangeCreateAction
	{
		public ChangeAction Action => ChangeAction.Transform;

		public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
			=> crashArgs.Args is RhinoTransformObjectsEventArgs;

		public bool TryConvert(object sender, CreateRecieveArgs crashArgs, out IEnumerable<IChange> changes)
		{
			changes = Array.Empty<IChange>();
			if (crashArgs.Args is not RhinoTransformObjectsEventArgs rargs) return false;

			var _user = crashArgs.Doc.Users.CurrentUser.Name;
			var transform = rargs.Transform.ToCrash();

			changes = getTransforms(transform, _user, rargs.Objects);

			return true;
		}

		private IEnumerable<IChange> getTransforms(CTransform transform, string userName, IEnumerable<RhinoObject> rhinoObjects)
		{
			foreach (var rhinoObject in rhinoObjects)
			{
				if (!rhinoObject.TryGetChangeId(out var changeId)) continue;

				var transChange = TransformChange.CreateNew(transform, userName, changeId);
				yield return transChange;
			}
		}

	}

}
