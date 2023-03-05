using Crash.Common.Events;

namespace Crash.Common.Changes
{
	public interface ICachedChange : IChange
	{

		public Action<CrashEventArgs> Draw { get; }

		public Action<CrashEventArgs> AddToDocument { get; }

		public Action<CrashEventArgs> RemoveFromDocument { get; }

	}

}
