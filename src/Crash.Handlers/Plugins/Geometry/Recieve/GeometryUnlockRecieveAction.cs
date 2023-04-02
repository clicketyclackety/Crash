﻿using Crash.Common.Changes;

namespace Crash.Handlers.Plugins.Geometry.Recieve
{
	internal sealed class GeometryUnlockRecieveAction : IChangeRecieveAction
	{
		/// <summary>The Action this ICreateAction responds to</summary>
		public ChangeAction Action => ChangeAction.Unlock;

		public void OnRecieve(CrashDoc crashDoc, Change recievedChange)
		{
			if (!crashDoc.CacheTable.TryGetValue(recievedChange.Id, out GeometryChange geomChange)) return;
			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(crashDoc);
			rhinoDoc.Objects.Unlock(geomChange.RhinoId, true);
		}
	}

}
