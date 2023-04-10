using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Events;
using Crash.Utils;

using Rhino.DocObjects;

namespace Crash.Handlers.Plugins.Geometry.Recieve
{
	internal sealed class GeometryRemoveRecieveAction : IChangeRecieveAction
	{
		/// <summary>The Action this ICreateAction responds to</summary>
		public ChangeAction Action => ChangeAction.Remove;

		public void OnRecieve(CrashDoc crashDoc, Change recievedChange)
		{
			IdleArgs idleArgs;
			IdleAction idleAction;

			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(crashDoc);
			if (!ChangeUtils.TryGetRhinoObject(recievedChange, out RhinoObject rhinoObject))
			{
				if (crashDoc.CacheTable.TryGetValue(recievedChange.Id, out GeometryChange change))
				{
					idleArgs = new IdleArgs(crashDoc, recievedChange);
					idleAction = new IdleAction(RemoveTemporaryFromDocument, idleArgs);
					crashDoc.Queue.AddAction(idleAction);
				}
				return;
			}

			idleArgs = new IdleArgs(crashDoc, recievedChange);
			idleAction = new IdleAction(RemoveFromDocument, idleArgs);
			crashDoc.Queue.AddAction(idleAction);
		}

		private void RemoveFromDocument(IdleArgs args)
		{
			if (!ChangeUtils.TryGetRhinoObject(args.Change, out RhinoObject rhinoObject)) return;
			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(args.Doc);

			rhinoDoc.Objects.Delete(rhinoObject, true);
			args.Doc.CacheTable.RemoveChange(args.Change.Id);
		}

		private void RemoveTemporaryFromDocument(IdleArgs args)
		{
			args.Doc.CacheTable.RemoveChange(args.Change.Id);
		}

	}

}
