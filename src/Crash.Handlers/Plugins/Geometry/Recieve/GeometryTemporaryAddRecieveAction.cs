using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Events;

namespace Crash.Handlers.Plugins.Geometry.Recieve
{
	internal sealed class GeometryTemporaryAddRecieveAction : IChangeRecieveAction
	{
		/// <summary>The Action this ICreateAction responds to</summary>
		public ChangeAction Action => ChangeAction.Add | ChangeAction.Temporary;

		public void OnRecieve(CrashDoc crashDoc, Change recievedChange)
		{
			if (recievedChange.Owner.Equals(crashDoc.Users.CurrentUser.Name))
			{
				var add = new GeometryAddRecieveAction();
				add.OnRecieve(crashDoc, recievedChange);
			}
			else
			{
				var geomChange = new GeometryChange(recievedChange);
				var changeArgs = new IdleArgs(crashDoc, geomChange);
				var displayAction = new IdleAction(AddToDocument, changeArgs);
				crashDoc.Queue.AddAction(displayAction);
			}
		}

		private void AddToDocument(IdleArgs args)
		{
			if (args.Change is not GeometryChange geomChange) return;

			args.Doc.CacheTable.UpdateChangeAsync(geomChange);
		}

	}

}
