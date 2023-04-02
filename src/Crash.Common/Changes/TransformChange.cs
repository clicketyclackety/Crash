using System.Text.Json;

using Crash.Geometry;

namespace Crash.Common.Changes
{

	public struct TransformChange : IChange
	{

		public CTransform Transform { get; private set; }

		public DateTime Stamp { get; private set; }

		public Guid Id { get; private set; }

		public string? Owner { get; private set; }

		public string? Payload { get; private set; }

		public string Type => $"{nameof(Crash)}.{nameof(TransformChange)}";

		public ChangeAction Action { get; set; }


		public TransformChange() { }

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
