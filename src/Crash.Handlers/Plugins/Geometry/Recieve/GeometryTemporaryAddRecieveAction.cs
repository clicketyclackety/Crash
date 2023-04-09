using Crash.Common.Changes;
using Crash.Common.Document;

namespace Crash.Handlers.Plugins.Geometry.Recieve
{
	internal sealed class GeometryTemporaryAddRecieveAction : IChangeRecieveAction
	{
		/// <summary>The Action this ICreateAction responds to</summary>
		public ChangeAction Action => ChangeAction.Add | ChangeAction.Temporary;

		public void OnRecieve(CrashDoc crashDoc, Change recievedChange)
		{
			if (recievedChange.Owner.Equals(crashDoc.Users.CurrentUser.Name))
			{
				var add = new GeometryAddRecieveAction();
				add.OnRecieve(crashDoc, recievedChange);
			}
			else
			{
				var geomChange = new GeometryChange(recievedChange);
				crashDoc.CacheTable.UpdateChangeAsync(geomChange);
			}
		}

	}

}
