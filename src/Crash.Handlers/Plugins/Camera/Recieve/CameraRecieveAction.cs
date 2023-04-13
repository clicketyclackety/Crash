using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Events;

using Rhino;
using Rhino.Geometry;

namespace Crash.Handlers.Plugins.Camera.Recieve
{

	/// <summary>Handles recieving a camera from the Server</summary>
	internal sealed class CameraRecieveAction : IChangeRecieveAction
	{

		/// <inheritdoc/>
		public ChangeAction Action => ChangeAction.Camera;

		/// <inheritdoc/>
		public async Task OnRecieveAsync(CrashDoc crashDoc, Change recievedChange)
		{
			var cameraArgs = new IdleArgs(crashDoc, recievedChange);
			var cameraAction = new IdleAction(AddToDocument, cameraArgs);
			await crashDoc.Queue.AddActionAsync(cameraAction);
		}

		private void AddToDocument(IdleArgs args)
		{
			var convertedChange = new CameraChange(args.Change);
			args.Doc.Cameras.TryAddCamera(convertedChange);
			args.Doc.Users.Add(args.Change.Owner);

			if (args.Doc.Users.Get(args.Change.Owner).Camera == CameraState.Follow)
			{
				FollowCamera(convertedChange);
			}
		}

		private void FollowCamera(CameraChange change)
		{
			var activeView = RhinoDoc.ActiveDoc.Views.ActiveView;

			Point3d cameraTarget = change.Camera.Target.ToRhino();
			Point3d cameraLocation = change.Camera.Location.ToRhino();

			activeView.ActiveViewport.SetCameraLocations(cameraTarget, cameraLocation);
		}

	}

}
