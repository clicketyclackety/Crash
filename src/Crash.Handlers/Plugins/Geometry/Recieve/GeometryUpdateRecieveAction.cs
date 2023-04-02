using Crash.Common.Changes;

namespace Crash.Handlers.Plugins.Geometry.Recieve
{
	internal sealed class GeometryUpdateRecieveAction : IChangeRecieveAction
	{
		/// <summary>The Action this ICreateAction responds to</summary>
		public ChangeAction Action => ChangeAction.Update;

		public void OnRecieve(CrashDoc crashDoc, Change recievedChange)
		{
			if (!crashDoc.CacheTable.TryGetValue(recievedChange.Id, out GeometryChange geomChange)) return;
			// geomChange.AddAction(recievedChange.Action);
			// Get Update Data
		}
	}

}
