using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Events;

namespace Crash.Handlers.Plugins.Geometry.Recieve
{

	/// <summary>Handles temporary objects from the server</summary>
	internal sealed class GeometryTemporaryAddRecieveAction : IChangeRecieveAction
	{

		/// <inheritdoc/>
		public ChangeAction Action => ChangeAction.Add | ChangeAction.Temporary;

		/// <inheritdoc/>
		public async Task OnRecieveAsync(CrashDoc crashDoc, Change recievedChange)
		{
			if (recievedChange.Owner.Equals(crashDoc.Users.CurrentUser.Name))
			{
				var add = new GeometryAddRecieveAction();
				await add.OnRecieveAsync(crashDoc, recievedChange);
			}
			else
			{
				var geomChange = new GeometryChange(recievedChange);
				var changeArgs = new IdleArgs(crashDoc, geomChange);
				var displayAction = new IdleAction(AddToDocument, changeArgs);
				await crashDoc.Queue.AddActionAsync(displayAction);
			}
		}

		private void AddToDocument(IdleArgs args)
		{
			if (args.Change is not GeometryChange geomChange) return;

			args.Doc.CacheTable.UpdateChangeAsync(geomChange);
		}

	}

}
