namespace Crash.Handlers.Plugins
{
	public interface ICrashPlugin
	{
		public string Name { get; }
		public Guid Id { get; }
	}

}
