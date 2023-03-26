/// <summary>
/// If you need to implement a Custom ChangeAction
/// It's best to implement all of the defaults here in your custom enum,
/// and then implement your own after these using negative numbers.
/// I have added unit tests into this repo to prove this is safe.
/// </summary>
[Flags]
public enum ChangeAction
{
	/// <summary>No Change</summary>
	None = 0,

	/// <summary>Add Change</summary>
	Add = 1 << 1,
	/// <summary>Remove Change</summary>
	Remove = 1 << 2,

	/// <summary>Misc. Update Change</summary>
	Update = 1 << 3,

	/// <summary>Transform Change</summary>
	Transform = 1 << 4,

	/// <summary>A Locking Change</summary>
	Lock = 1 << 5,
	/// <summary>An Unlocking Change</summary>
	Unlock = 1 << 6,

	/// <summary>A Temporary Change</summary>
	Temporary = 1 << 7, //?

	/// <summary>A Camera Change</summary>
	Camera = 1 << 8,

}
