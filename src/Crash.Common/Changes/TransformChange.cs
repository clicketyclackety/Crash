using System.Text.Json;

using Crash.Geometry;

namespace Crash.Common.Changes
{

	public struct TransformChange : IChange
	{
		IChange Change { get; set; }

		public CTransform Transform { get; private set; }

		public DateTime Stamp => Change.Stamp;

		public Guid Id => Change.Id;

		public string? Owner => Change.Owner;

		public string? Payload => Change.Payload;

		public string Type { get; } = nameof(TransformChange);

		public ChangeAction Action
		{
			get => Change.Action;
			[Obsolete("For Deserialization only", true)]
			set => Change.Action = value;
		}

		public TransformChange(IChange change)
		{
			Change = change;
			if (string.IsNullOrEmpty(Change.Payload))
			{
				throw new ArgumentNullException($"Payload is invalid, {nameof(Change.Payload)}");
			}

			Transform = JsonSerializer.Deserialize<CTransform>(Change.Payload);
		}

		public static TransformChange CreateNew(CTransform transform, string userName)
		{
			string json = JsonSerializer.Serialize(transform, Serialization.Options.Default);
			IChange tempChange = new Change(Guid.NewGuid(), userName, json);
			return new TransformChange(tempChange);
		}

	}

}
