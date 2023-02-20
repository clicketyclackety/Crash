using System.Collections;
using System.Text.Json;
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

		private Dictionary<string, FixedSizedQueue<Camera>> cameraLocations;

		public CameraTable(CrashDoc hostDoc)
		{
			cameraLocations = new Dictionary<string, FixedSizedQueue<Camera>>();
			crashDoc = hostDoc;
		}


		internal void OnCameraChange(string userName, Change cameraChange)
		{
			if (string.IsNullOrEmpty(userName)) return;
			var user = crashDoc.Users.Get(userName);
			if (null == user) return;

			var newCamera = JsonSerializer.Deserialize<Camera>(cameraChange.Payload);
			if (!newCamera.IsValid()) return;

			CameraIsInvalid = true;

			// Add to Cache
			if (cameraLocations.TryGetValue(userName, out var previousCameras))
			{
				previousCameras.Enqueue(newCamera);
			}
			else
			{
				var newStack = new FixedSizedQueue<Camera>(MAX_CAMERAS_IN_QUEUE);
				newStack.Enqueue(newCamera);
				cameraLocations.Add(userName, newStack);
			}
		}

		public Dictionary<string, Camera> GetActiveCameras()
		{
			return cameraLocations.ToDictionary(cl => cl.Key, cl => cl.Value.FirstOrDefault());
		}

		public bool TryGetCamera(string userName, out FixedSizedQueue<Camera> cameras)
			=> cameraLocations.TryGetValue(userName, out cameras);

		public IEnumerator<Camera> GetEnumerator() => cameraLocations.Values.SelectMany(c => c).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	}

}
