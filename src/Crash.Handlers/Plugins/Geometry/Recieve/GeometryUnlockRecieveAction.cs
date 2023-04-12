using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Events;
using Crash.Utils;

namespace Crash.Handlers.Plugins.Geometry.Recieve
{
	internal sealed class GeometryUnlockRecieveAction : IChangeRecieveAction
	{
		/// <summary>The Action this ICreateAction responds to</summary>
		public ChangeAction Action => ChangeAction.Unlock;

		public async Task OnRecieveAsync(CrashDoc crashDoc, Change recievedChange)
		{
			var changeArgs = new IdleArgs(crashDoc, recievedChange);
			var lockAction = new IdleAction(LockChange, changeArgs);
			await crashDoc.Queue.AddActionAsync(lockAction);
		}

		private void LockChange(IdleArgs args)
		{
			if (!ChangeUtils.TryGetRhinoObject(args.Change, out var rhinoObject)) return;
			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(args.Doc);
			rhinoDoc.Objects.Unlock(rhinoObject, true);
		}

	}

}
