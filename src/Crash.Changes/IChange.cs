
/// <summary>
/// The IChange interface for changes to be sent to the database and shared.
/// </summary>
public interface IChange
{
	/// <summary>
	/// The Date and Time stamp
	/// </summary>
	public DateTime Stamp { get; }

	/// <summary>
	/// The unique id of the change
	/// </summary>
	public Guid Id { get; }

	/// <summary>
	/// The owner name of the change
	/// </summary>
	public string? Owner { get; }

	/// <summary>
	/// The payload, the object that changed serialized as string
	/// </summary>
	public string? Payload { get; }

	/// <summary>
	/// Action can be any enum, but by default uses the ChangeAction
	/// </summary>
	public int Action { get; set; }
}
