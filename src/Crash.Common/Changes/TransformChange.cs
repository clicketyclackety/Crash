using System.Text.Json;

using Crash.Geometry;

namespace Crash.Common.Changes
{

	/// <summary>Captures a Transformation Change</summary>
	public struct TransformChange : IChange
	{
		/// <summary>The CTransform</summary>
		public CTransform Transform { get; private set; }

		/// <inheritdoc/>
		public DateTime Stamp { get; private set; }

		/// <inheritdoc/>
		public Guid Id { get; private set; }

		/// <inheritdoc/>
		public string? Owner { get; private set; }

		/// <inheritdoc/>
		public string? Payload { get; private set; }

		/// <inheritdoc/>
		public string Type => $"{nameof(Crash)}.{nameof(TransformChange)}";

		/// <inheritdoc/>
		public ChangeAction Action { get; set; }

		/// <summary>Empty Constructor</summary>
		public TransformChange() { }

		/// <summary>IChange wrapping Constructor</summary>
		public TransformChange(IChange change)
		{
			if (string.IsNullOrEmpty(change.Payload))
			{
				throw new ArgumentNullException($"Payload is invalid, {nameof(Change.Payload)}");
			}

			Transform = JsonSerializer.Deserialize<CTransform>(change.Payload);

			Payload = change.Payload;
			Owner = change.Owner;
			Stamp = change.Stamp;
			Id = change.Id;
		}

		/// <summary>Creates a Transform Cahnge</summary>
		public static TransformChange CreateNew(CTransform transform, string userName, Guid id)
		{
			string json = JsonSerializer.Serialize(transform, Serialization.Options.Default);
			return new TransformChange()
			{
				Id = id,
				Owner = userName,
				Payload = json,
				Stamp = DateTime.UtcNow,
			};
		}

	}

}
