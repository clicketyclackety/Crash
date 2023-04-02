using Crash.Common.Changes;
using Crash.Common.Document;

namespace Crash.Handlers.Plugins.Camera.Recieve
{
	internal sealed class CameraRecieveAction : IChangeRecieveAction
	{

		public ChangeAction Action => ChangeAction.Camera;

		public void OnRecieve(CrashDoc crashDoc, Change recievedChange)
		{
			var convertedChange = new CameraChange(recievedChange);
			crashDoc.Cameras.TryAddCamera(convertedChange);
		}
	}

}
