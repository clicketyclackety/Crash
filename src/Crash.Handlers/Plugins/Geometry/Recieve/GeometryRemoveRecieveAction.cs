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
			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(crashDoc);
			if (!ChangeUtils.TryGetRhinoObject(recievedChange, out RhinoObject rhinoObject)) return;

			var idleArgs = new IdleArgs(crashDoc, recievedChange);
			var bakeAction = new IdleAction(RemoveFromDocument, idleArgs);
			crashDoc.Queue.AddAction(bakeAction);
		}

		private void RemoveFromDocument(IdleArgs args)
		{
			if (!ChangeUtils.TryGetRhinoObject(args.Change, out RhinoObject rhinoObject)) return;
			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(args.Doc);

			rhinoDoc.Objects.Delete(rhinoObject, true);
			args.Doc.CacheTable.RemoveChange(args.Change.Id);
		}
	}

}
