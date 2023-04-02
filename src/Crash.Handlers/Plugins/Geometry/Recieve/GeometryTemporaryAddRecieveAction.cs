using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Events;
using Crash.Handlers.Args;

namespace Crash.Handlers.Plugins.Geometry.Recieve
{
	internal sealed class GeometryTemporaryAddRecieveAction : IChangeRecieveAction
	{
		/// <summary>The Action this ICreateAction responds to</summary>
		public ChangeAction Action => ChangeAction.Add | ChangeAction.Temporary;

		public void OnRecieve(CrashDoc crashDoc, Change recievedChange)
		{
			var geomChange = new GeometryChange(recievedChange);

			var changeArgs = new ChangeArgs(crashDoc, geomChange);
			var bakeAction = new IdleAction(AddToDocument, changeArgs);
			crashDoc.Queue.AddAction(bakeAction);
		}

		private void AddToDocument(IdleArgs args)
		{
			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(args.Doc);
			if (args.Change is not GeometryChange geomChange) return;

			args.Doc.CacheTable.UpdateChangeAsync(geomChange);
		}

	}

}
