using Crash.Common.Events;

namespace Crash.Common.Changes
{
	public interface ICachedChange : IChange
	{
		/// <summary>
		/// TODO: This is currently never getting used! Implement correctly.
		/// </summary>
		public Action<CrashEventArgs> Draw { get; }

		public Action<CrashEventArgs> AddToDocument { get; }

		public Action<CrashEventArgs> RemoveFromDocument { get; }

	}

}
