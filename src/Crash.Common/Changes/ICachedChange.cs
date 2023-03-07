using Crash.Common.Events;

namespace Crash.Common.Changes
{
	/// <summary>
	/// ICachedChange actions that define how the object is handled in the application
	/// </summary>
	public interface ICachedChange : IChange
	{
		/// <summary>
		/// This defines how the change will be drawn
		/// </summary>
		public Action<CrashEventArgs> Draw { get; }

		/// <summary>
		/// Defines how the change will be added to the document
		/// </summary>
		public Action<CrashEventArgs> AddToDocument { get; }

		/// <summary>
		/// Defines how the change will be removed from the document
		/// </summary>
		public Action<CrashEventArgs> RemoveFromDocument { get; }

	}

}
