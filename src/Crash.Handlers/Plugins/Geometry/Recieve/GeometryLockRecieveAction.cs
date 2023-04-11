using Crash.Common.Changes;
using Crash.Common.Document;

namespace Crash.Handlers.Plugins.Geometry.Recieve
{
	internal sealed class GeometryLockRecieveAction : IChangeRecieveAction
	{
		/// <summary>The Action this ICreateAction responds to</summary>
		public ChangeAction Action => ChangeAction.Lock;

		public async Task OnRecieveAsync(CrashDoc crashDoc, Change recievedChange)
		{
			if (!crashDoc.CacheTable.TryGetValue(recievedChange.Id, out GeometryChange geomChange)) return;
			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(crashDoc);
			rhinoDoc.Objects.Lock(geomChange.RhinoId, true);
		}
	}

}
