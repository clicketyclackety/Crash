using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Handlers.InternalEvents;
using Crash.Utils;

using Rhino.Geometry;

namespace Crash.Handlers.Plugins.Geometry.Create
{

	/// <summary>Captures Creating </summary>
	internal sealed class GeometryCreateAction : IChangeCreateAction
	{
		/// <inheritdoc/>
		public ChangeAction Action => ChangeAction.Add | ChangeAction.Temporary;

		/// <inheritdoc/>
		public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
			=> crashArgs.Args is CrashObjectEventArgs rargs &&
			   rargs.Geometry is not null;

		/// <inheritdoc/>
		public bool TryConvert(object sender, CreateRecieveArgs crashArgs, out IEnumerable<IChange> changes)
		{
			if (crashArgs.Args is CrashObjectEventArgs cargs)
			{
				changes = CreateChangesFromArgs(crashArgs.Doc, cargs.RhinoId, cargs.Geometry);
				return true;
			}

			changes = Array.Empty<IChange>();
			return false;
		}

		private IEnumerable<IChange> CreateChangesFromArgs(CrashDoc crashDoc, Guid rhinoId, GeometryBase geometry)
		{
			var _user = crashDoc.Users.CurrentUser.Name;
			var change = GeometryChange.CreateNew(_user, geometry);

			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(crashDoc);
			var rhinoObject = rhinoDoc.Objects.FindId(rhinoId);
			ChangeUtils.SyncHost(rhinoObject, change);

			change.Action = Action;

			yield return change;
		}

	}

}
