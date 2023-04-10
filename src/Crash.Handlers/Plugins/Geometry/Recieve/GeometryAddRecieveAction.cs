using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Events;
using Crash.Utils;

using Rhino.DocObjects;

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
			if (IsDuplicate(crashDoc, geomChange)) return;

			var changeArgs = new IdleArgs(crashDoc, geomChange);
			var bakeAction = new IdleAction(AddToDocument, changeArgs);
			crashDoc.Queue.AddAction(bakeAction);
		}

		// Prevents issues with the same user logged in twice
		private bool IsDuplicate(CrashDoc crashDoc, IChange change)
		{
			bool isNotInit = !crashDoc.CacheTable.IsInit;
			bool isByCurrentUser = change.Owner.Equals(crashDoc.Users.CurrentUser.Name);
			return isNotInit && isByCurrentUser;
		}

		private void AddToDocument(IdleArgs args)
		{
			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(args.Doc);
			if (args.Change is not GeometryChange geomChange) return;

			Guid rhinoId = rhinoDoc.Objects.Add(geomChange.Geometry);
			RhinoObject rhinoObject = rhinoDoc.Objects.FindId(rhinoId);
			ChangeUtils.SyncHost(rhinoObject, geomChange);
		}

	}

}
