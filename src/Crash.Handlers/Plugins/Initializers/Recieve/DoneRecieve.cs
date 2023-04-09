using Crash.Changes.Extensions;
using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Handlers.Plugins.Geometry.Recieve;

namespace Crash.Handlers.Plugins.Initializers.Recieve
{
	internal class DoneRecieve : IChangeRecieveAction
	{

		public ChangeAction Action => ChangeAction.None;

		public void OnRecieve(CrashDoc crashDoc, Change recievedChange)
		{
			var changes = crashDoc.CacheTable.GetChanges().Where(c => c.Owner == recievedChange.Owner);

			crashDoc.CacheTable.SomeoneIsDone = true;
			try
			{
				var add = new GeometryAddRecieveAction();
				foreach (var change in changes)
				{
					if (!crashDoc.CacheTable.TryGetValue(change.Id,
						out GeometryChange geomChange)) continue;

					geomChange.RemoveAction(ChangeAction.Temporary);
					geomChange.AddAction(ChangeAction.Add);

					add.OnRecieve(crashDoc, geomChange);
					crashDoc.CacheTable.RemoveChange(change.Id);
				}
			}
			finally
			{
				EventHandler? _event = null;
				_event = (sender, args) =>
				{
					crashDoc.CacheTable.SomeoneIsDone = false;
					crashDoc.Queue.OnCompletedQueue -= _event;
				};

				crashDoc.Queue.OnCompletedQueue += _event;
			}
		}
	}

}
