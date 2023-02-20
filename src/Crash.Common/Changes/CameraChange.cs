using System.Text.Json;
using Crash.Common.Events;
using Crash.Common.View;

namespace Crash.Common.Changes
{

	public sealed class CameraChange : ICachedChange
	{
		IChange Change { get; set; }

		public Camera Camera { get; private set; }

		public DateTime Stamp => Change.Stamp;

		public Guid Id => Change.Id;

		public string? Owner => Change.Owner;

		public string? Payload => Change.Payload;

		public int Action
		{
			get => Change.Action;
			[Obsolete("For Deserialization only", true)]
			set => Change.Action = value;
		}

		public CameraChange()
		{

		}

		private CameraChange(IChange change)
		{
			Change = change;

			if (string.IsNullOrEmpty(Change.Payload))
			{
				throw new ArgumentNullException($"Payload is invalid, {nameof(Change.Payload)}");
			}

			var camera = JsonSerializer.Deserialize<Camera>(Change.Payload);

			if (!camera.IsValid())
			{
				throw new JsonException("Could not deserialize Camera");
			}

			Camera = camera;
		}

		public static CameraChange CreateNew(Camera camera, string userName)
		{
			string json = JsonSerializer.Serialize(camera, Serialization.Options.Default);
			global::IChange cameraChange = new Change(Guid.NewGuid(), userName, json);
			return new CameraChange(cameraChange);
		}

		public Action<CrashEventArgs> Draw { get; set; }

		public Action<CrashEventArgs> AddToDocument { get; set; }

		public Action<CrashEventArgs> RemoveFromDocument { get; set; }

	}

}
