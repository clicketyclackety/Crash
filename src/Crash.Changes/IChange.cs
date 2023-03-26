public interface IChange
{

	/// <summary>The time of creation</summary>
	public DateTime Stamp { get; }

	/// <summary>The Id of the Change</summary>
	public Guid Id { get; }

	/// <summary>The originator of the Change</summary>
	public string? Owner { get; }

	/// <summary>Any related payload data</summary>
	public string? Payload { get; }

	/// <summary>The Payload Type</summary>
	public string Type { get; }

	/// <summary>The type of Change. See ChangeAction.</summary>
	public ChangeAction Action { get; set; }

}
