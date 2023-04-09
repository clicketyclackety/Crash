using Crash.Common.Document;
using Crash.Utils;

using Rhino.DocObjects;

namespace Crash.Handlers.Plugins.Geometry.Recieve
{
	internal sealed class GeometryUnlockRecieveAction : IChangeRecieveAction
	{
		/// <summary>The Action this ICreateAction responds to</summary>
		public ChangeAction Action => ChangeAction.Unlock;

		public void OnRecieve(CrashDoc crashDoc, Change recievedChange)
		{
			if (!ChangeUtils.TryGetRhinoObject(recievedChange, out RhinoObject rhinoObject)) return;

			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(crashDoc);
			rhinoDoc.Objects.Unlock(rhinoObject, true);
		}
	}

}
