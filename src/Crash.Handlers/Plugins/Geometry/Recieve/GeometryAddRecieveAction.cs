using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Events;
using Crash.Utils;

namespace Crash.Handlers.Plugins.Geometry.Recieve
{

	/// <summary>Handles recieving of Geometry</summary>
	internal sealed class GeometryAddRecieveAction : IChangeRecieveAction
	{

		/// <inheritdoc/>
		public ChangeAction Action => ChangeAction.Add;

		/// <inheritdoc/>
		public async Task OnRecieveAsync(CrashDoc crashDoc, Change recievedChange)
			=> await OnRecieveAsync(crashDoc, new GeometryChange(recievedChange));

		/// <summary>Handles recieved Geometry Changes</summary>
		public async Task OnRecieveAsync(CrashDoc crashDoc, GeometryChange geomChange)
		{
			if (GeometryAddRecieveAction.IsDuplicate(crashDoc, geomChange)) return;

			var changeArgs = new IdleArgs(crashDoc, geomChange);
			var bakeAction = new IdleAction(AddToDocument, changeArgs);
			await crashDoc.Queue.AddActionAsync(bakeAction);
		}

		// Prevents issues with the same user logged in twice
		private static bool IsDuplicate(CrashDoc crashDoc, IChange change)
		{
			bool isNotInit = !crashDoc.CacheTable.IsInit;
			bool isByCurrentUser = change.Owner.Equals(crashDoc.Users.CurrentUser.Name, StringComparison.Ordinal);
			return isNotInit && isByCurrentUser;
		}

		private void AddToDocument(IdleArgs args)
		{
			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(args.Doc);
			if (args.Change is not GeometryChange geomChange) return;

			args.Doc.CacheTable.IsInit = true;
			try
			{
				Guid rhinoId = rhinoDoc.Objects.Add(geomChange.Geometry);
				Rhino.DocObjects.RhinoObject rhinoObject = rhinoDoc.Objects.FindId(rhinoId);
				ChangeUtils.SyncHost(rhinoObject, geomChange);
			}
			finally
			{
				args.Doc.CacheTable.IsInit = false;
			}
		}

	}

}
