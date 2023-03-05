public interface IChange
{

	public DateTime Stamp { get; }

	public Guid Id { get; }

	public string? Owner { get; }

	public string? Payload { get; }

	/// <summary>
	/// Action can be any enum, but by default uses the ChangeAction
	/// </summary>
	public int Action { get; set; }
}
