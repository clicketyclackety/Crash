namespace Crash.Changes
{
	/// <summary>Provides a reliable class for communication</summary>
	public sealed class Change : IChange
	{

		public DateTime Stamp { get; set; }

		public Guid Id { get; set; }

		public string? Owner { get; set; }

		public string? Payload { get; set; }

		public int Action { get; set; }

		// For Deserialization Only
		public Change() { }


		public Change(Guid id, string owner, string? payload)
		{
			Id = id;
			Owner = owner;
			Payload = payload;
			Stamp = DateTime.UtcNow;
		}

		public Change(IChange speck)
		{
			Stamp = speck.Stamp;
			Id = speck.Id;
			Owner = speck.Owner;
			Payload = speck.Payload;
			Action = speck.Action;
		}

		public static Change CreateEmpty()
		{
			return new Change()
			{
				Id = Guid.NewGuid()
			};
		}

	}

}
