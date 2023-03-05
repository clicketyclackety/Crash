namespace Crash.Changes
{

	/// <summary>Provides a reliable class for communication</summary>
	public sealed class Change : IChange, IEquatable<Change>
	{

		/// <summary>The time of creation</summary>
		public DateTime Stamp { get; set; }

		/// <summary>The Id of the Change</summary>
		public Guid Id { get; set; }

		/// <summary>The originator of the Change</summary>
		public string? Owner { get; set; }

		/// <summary>Any related payload data</summary>
		public string? Payload { get; set; }

		/// <summary>The type of Change. See ChangeAction.</summary>
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


		public override int GetHashCode() => HashCode.Combine(Id, Owner, Action, Payload);

		public bool Equals(Change other)
			=> other?.GetHashCode() == GetHashCode();

		public override bool Equals(object obj)
		{
			if (obj is not Change change) return false;
			return Equals(change);
		}
	}

}
