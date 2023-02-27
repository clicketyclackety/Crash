/// <summary>
/// If you need to a Custom ChangeAction
/// It's best to implement all of the defaults here in your custom enum,
/// and then implement your own after these using negative numbers.
/// I have added unit tests into this repo to prove this is safe.
/// </summary>
[Flags]
public enum ChangeAction
{
	None = 1 << 0,

	Add = 1 << 1,
	Remove = 1 << 2,

	Update = 1 << 4,

	Transform = 1 << 8,

	Lock = 1 << 16,
	Unlock = 1 << 32,

	Temporary = 1 << 64, //?

	Camera = 1 << 128,

}
