namespace Crash.Changes
{

	/// <summary>Provides a reliable, reusable class for communication</summary>
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


		#region Constructors

		/// <summary>Empty Constructor</summary>
		public Change() { }

		/// <summary>Creates a new fresh Change</summary>
		public Change(Guid id, string owner, string? payload)
		{
			Id = id;
			Owner = owner;
			Payload = payload;
			Stamp = DateTime.UtcNow;
		}

		/// <summary>Creates a transmittable Change from an IChange</summary>
		public Change(IChange speck)
		{
			Stamp = speck.Stamp;
			Id = speck.Id;
			Owner = speck.Owner;
			Payload = speck.Payload;
			Action = speck.Action;
		}

		public static Change CreateEmpty(Guid id = default)
		{
			return new Change()
			{
				Id = id == Guid.Empty ? Guid.NewGuid() : id
			};
		}

		#endregion


		/// <inheritdoc/>
		public override int GetHashCode() => HashCode.Combine(Id, Owner, Action, Payload);

		/// <summary>Tests for equality of two changes</summary>
		public bool Equals(Change other)
			=> other?.GetHashCode() == GetHashCode();

		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			if (obj is not Change change) return false;
			return Equals(change);
		}

	}

}
