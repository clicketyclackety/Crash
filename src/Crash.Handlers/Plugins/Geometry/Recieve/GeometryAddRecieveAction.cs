using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Events;

namespace Crash.Handlers.Plugins.Geometry.Recieve
{
	internal sealed class GeometryAddRecieveAction : IChangeRecieveAction
	{
		/// <summary>The Action this ICreateAction responds to</summary>
		public ChangeAction Action => ChangeAction.Add;

		public void OnRecieve(CrashDoc crashDoc, Change recievedChange)
			=> OnRecieve(crashDoc, new GeometryChange(recievedChange));

		public void OnRecieve(CrashDoc crashDoc, GeometryChange geomChange)
		{
			var changeArgs = new IdleArgs(crashDoc, geomChange);
			var bakeAction = new IdleAction(AddToDocument, changeArgs);
			crashDoc.Queue.AddAction(bakeAction);
		}

		private void AddToDocument(IdleArgs args)
		{
			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(args.Doc);
			if (args.Change is not GeometryChange geomChange) return;

			rhinoDoc.Objects.Add(geomChange.Geometry);
		}

	}

}
