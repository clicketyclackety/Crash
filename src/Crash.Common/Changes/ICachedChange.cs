using Crash.Common.Events;

namespace Crash.Common.Changes
{
	public interface ICachedChange : IChange
	{

		public Action<IdleArgs> Draw { get; }

		public Action<IdleArgs> AddToDocument { get; }

		public Action<IdleArgs> RemoveFromDocument { get; }

	}

}
