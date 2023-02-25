using System.Collections;
using System.Text.Json;

using Crash.Common.Changes;
using Crash.Common.Collections;
using Crash.Common.Document;
using Crash.Common.View;

namespace Crash.Common.Tables
{

	public sealed class CameraTable : IEnumerable<Camera>
	{
		public const int MAX_CAMERAS_IN_QUEUE = 3;

		public bool CameraIsInvalid = false;

		private CrashDoc crashDoc;

		private Dictionary<User, FixedSizedQueue<Camera>> cameraLocations;


		public CameraTable(CrashDoc hostDoc)
		{
			cameraLocations = new Dictionary<User, FixedSizedQueue<Camera>>();
			crashDoc = hostDoc;
		}


		public void OnCameraChange(string userName, Change cameraChange)
		{
			if (string.IsNullOrEmpty(userName)) return;

			var user = crashDoc.Users.Get(userName);
			if (!user.IsValid()) return;

			var newCamera = JsonSerializer.Deserialize<Camera>(cameraChange.Payload);
			if (!newCamera.IsValid()) return;

			CameraIsInvalid = true;

			// Add to Cache
			if (cameraLocations.TryGetValue(user, out var previousCameras))
			{
				previousCameras.Enqueue(newCamera);
			}
			else
			{
				var newStack = new FixedSizedQueue<Camera>(MAX_CAMERAS_IN_QUEUE);
				newStack.Enqueue(newCamera);
				cameraLocations.Add(user, newStack);
			}
		}

		public Dictionary<User, Camera> GetActiveCameras()
		{
			return cameraLocations.ToDictionary(cl => cl.Key, cl => cl.Value.FirstOrDefault());
		}

		public bool TryAddCamera(CameraChange cameraChange)
		{
			User user = new User(cameraChange.Owner);
			FixedSizedQueue<Camera> queue;

			if (!cameraLocations.ContainsKey(user))
			{
				queue = new FixedSizedQueue<Camera>(MAX_CAMERAS_IN_QUEUE);
				queue.Enqueue(cameraChange.Camera);
				cameraLocations.Add(user, queue);
			}
			else
			{
				if (!cameraLocations.TryGetValue(user, out queue)) return false;
				queue.Enqueue(cameraChange.Camera);
			}

			return true;
		}

		public void TryAddCamera(IEnumerable<CameraChange> cameraChanges)
		{
			foreach (CameraChange camaeraChange in cameraChanges.OrderBy(cam => cam.Stamp))
			{
				TryAddCamera(camaeraChange);
			}
		}

		public bool TryGetCamera(User user, out FixedSizedQueue<Camera> cameras)
		{
			bool test = cameraLocations.TryGetValue(user, out cameras);
			;
			return test;
		}

		public IEnumerator<Camera> GetEnumerator() => cameraLocations.Values.SelectMany(c => c).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	}

}
