namespace Crash.Server
{
	/// <summary>
	/// EndPoints Interface
	/// </summary>
	public interface ICrashClient
	{
		Task Update(string user, Guid id, Change Change); // Change to Just Change
		Task Add(string user, Change Change); // Change to just Change
		Task Delete(string user, Guid id); // Change to Just ID
		Task Done(string user);
		Task Select(string user, Guid id);
		Task Unselect(string user, Guid id);
		Task Initialize(Change[] Changes); // Change to IEnumerable?
		Task CameraChange(string user, Change Change); // Change to just Change
	}
}
