using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Events;

namespace Crash.Handlers.Plugins.Geometry.Recieve
{
	internal sealed class GeometryRemoveRecieveAction : IChangeRecieveAction
	{
		/// <summary>The Action this ICreateAction responds to</summary>
		public ChangeAction Action => ChangeAction.Remove;

		public void OnRecieve(CrashDoc crashDoc, Change recievedChange)
		{
			if (!crashDoc.CacheTable.TryGetValue(recievedChange.Id, out GeometryChange geomChange)) return;

			var idleArgs = new IdleArgs(crashDoc, geomChange);
			var bakeAction = new IdleAction(RemoveFromDocument, idleArgs);
			crashDoc.Queue.AddAction(bakeAction);
		}

		private void RemoveFromDocument(IdleArgs args)
		{
			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(args.Doc);
			if (args.Change is not GeometryChange geomChange) return;

			var RhinoId = geomChange.RhinoId;
			rhinoDoc.Objects.Delete(RhinoId, true);
			args.Doc.CacheTable.RemoveChange(args.Change.Id);
		}
	}

}
