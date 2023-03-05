using System.Collections;

using Crash.Common.Document;

namespace Crash.Events
{

	/// <summary>
	/// A Queue for running during the Rhino Idle Event.
	/// </summary>
	public sealed class IdleQueue : IEnumerable<IdleAction>, IDisposable
	{
		private ConcurrentQueue<IdleAction> idleQueue;
		private CrashDoc hostDoc;

		public IdleQueue(CrashDoc hostDoc)
		{
			this.hostDoc = hostDoc;
			idleQueue = new ConcurrentQueue<IdleAction>();
		}

		public void AddAction(IdleAction action)
		{
			idleQueue.Enqueue(action);
		}

		public void RunNextAction()
		{
			if (idleQueue.Count == 0) return;

			idleQueue.TryDequeue(out IdleAction action);
			action?.Invoke();

			if (0 == idleQueue.Count)
			{
				OnCompletedQueue?.Invoke(this, null);
			}
		}

		public int Count => idleQueue.Count;

		public IEnumerator<IdleAction> GetEnumerator() => idleQueue.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => idleQueue.GetEnumerator();

		public void Dispose()
		{
			// What to do with the events?
		}

		public event EventHandler OnCompletedQueue;

	}

}
