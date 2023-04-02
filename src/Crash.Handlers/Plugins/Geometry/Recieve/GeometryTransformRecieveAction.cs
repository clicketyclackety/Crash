using Crash.Common.Changes;

namespace Crash.Handlers.Plugins.Geometry.Recieve
{
	internal sealed class GeometryTransformRecieveAction : IChangeRecieveAction
	{
		/// <summary>The Action this ICreateAction responds to</summary>
		public ChangeAction Action => ChangeAction.Transform;

		public void OnRecieve(CrashDoc crashDoc, Change recievedChange)
		{
			if (!crashDoc.CacheTable.TryGetValue(recievedChange.Id, out GeometryChange geomChange)) return;
			var transChange = new TransformChange(recievedChange);
			var xform = transChange.Transform.ToRhino();
			geomChange.Geometry.Transform(xform);
		}
	}

}
