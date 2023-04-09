using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Handlers.InternalEvents;

using Rhino.Geometry;

namespace Crash.Handlers.Plugins.Geometry.Create
{

	internal sealed class GeometryCreateAction : IChangeCreateAction
	{

		public ChangeAction Action => ChangeAction.Add;

		public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
			=> crashArgs.Args is CrashObjectEventArgs rargs &&
			   rargs.Geometry is not null;

		public bool TryConvert(object sender, CreateRecieveArgs crashArgs, out IEnumerable<IChange> changes)
		{
			if (crashArgs.Args is CrashObjectEventArgs cargs)
			{
				changes = CreateChangesFromArgs(crashArgs.Doc, cargs.Geometry);
				return true;
			}

			changes = Array.Empty<IChange>();
			return false;
		}

		private IEnumerable<IChange> CreateChangesFromArgs(CrashDoc crashDoc, GeometryBase geometry)
		{
			var _user = crashDoc.Users.CurrentUser.Name;
			yield return GeometryChange.CreateNew(_user, geometry);
		}

	}

}
