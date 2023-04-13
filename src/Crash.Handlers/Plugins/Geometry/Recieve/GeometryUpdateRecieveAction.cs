using Crash.Common.Changes;
using Crash.Common.Document;

namespace Crash.Handlers.Plugins.Geometry.Recieve
{

	/// <summary>Handles updates from the server</summary>
	internal sealed class GeometryUpdateRecieveAction : IChangeRecieveAction
	{

		/// <inheritdoc/>
		public ChangeAction Action => ChangeAction.Update;

		/// <inheritdoc/>
		public async Task OnRecieveAsync(CrashDoc crashDoc, Change recievedChange)
		{
			if (!crashDoc.CacheTable.TryGetValue(recievedChange.Id, out GeometryChange geomChange)) return;
			// geomChange.AddAction(recievedChange.Action);
			// Get Update Data
		}
	}

}
