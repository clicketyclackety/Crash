using Crash.Common.Changes;
using Crash.Geometry;
using Crash.Handlers.InternalEvents;

namespace Crash.Handlers.Plugins.Geometry.Create
{

	/// <summary>Handles Transform Changes</summary>
	internal sealed class GeometryTransformAction : IChangeCreateAction
	{

		/// <inheritdoc/>
		public ChangeAction Action => ChangeAction.Transform;

		/// <inheritdoc/>
		public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
			=> crashArgs.Args is CrashTransformEventArgs;

		/// <inheritdoc/>
		public bool TryConvert(object sender, CreateRecieveArgs crashArgs, out IEnumerable<IChange> changes)
		{
			changes = Array.Empty<IChange>();
			if (crashArgs.Args is not CrashTransformEventArgs cargs) return false;

			var _user = crashArgs.Doc.Users.CurrentUser.Name;
			var transform = cargs.Transform;

			changes = getTransforms(transform, _user, cargs.Objects);

			return true;
		}

		private IEnumerable<IChange> getTransforms(CTransform transform, string userName, IEnumerable<CrashObject> crashObjects)
		{
			foreach (var crashObject in crashObjects)
			{
				if (crashObject.ChangeId == Guid.Empty) continue;

				var transChange = TransformChange.CreateNew(transform, userName, crashObject.ChangeId);
				yield return transChange;
			}
		}

	}


}
