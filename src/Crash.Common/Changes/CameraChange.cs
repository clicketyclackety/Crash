using System.Text.Json;

using Crash.Common.View;

namespace Crash.Common.Changes
{

	/// <summary>Captues a Change of Camera</summary>
	public sealed class CameraChange : IChange
	{
		/// <summary>The Unique Name of this Change</summary>
		public const string ChangeName = $"{nameof(Crash)}.{nameof(CameraChange)}";

		/// <inheritdoc/>
		IChange Change { get; set; }

		public Camera Camera { get; set; }

		/// <inheritdoc/>
		public DateTime Stamp => Change.Stamp;

		/// <inheritdoc/>
		public Guid Id => Change.Id;

		/// <inheritdoc/>
		public string? Owner => Change.Owner;

		/// <inheritdoc/>
		public string? Payload => Change.Payload;

		/// <inheritdoc/>
		public ChangeAction Action
		{
			get => Change.Action;
			[Obsolete("For Deserialization only", true)]
			set => Change.Action = value;
		}

		/// <inheritdoc/>
		public string Type => ChangeName;

		/// <summary>Empty Constructor</summary>
		public CameraChange()
		{

		}

		/// <summary>IChange wrapping constructor</summary>
		public CameraChange(IChange change)
		{
			Change = change;
			Change.Action = ChangeAction.Camera;

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

		/// <summary>Creates a new Camera Change</summary>
		public static CameraChange CreateNew(Camera camera, string userName)
		{
			string json = JsonSerializer.Serialize(camera, Serialization.Options.Default);
			IChange cameraChange = new Change(Guid.NewGuid(), userName, json);
			return new CameraChange(cameraChange);
		}

	}

}
