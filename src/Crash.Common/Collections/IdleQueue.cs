using System.Collections;

using Crash.Common.Document;

namespace Crash.Events
{

	/// <summary>A Queue for running during the Rhino Idle Event.</summary>
	public sealed class IdleQueue : IEnumerable<IdleAction>
	{
		readonly ConcurrentQueue<IdleAction> _idleQueue;
		readonly CrashDoc _hostDoc;

		/// <summary>Constructs an Idle Queue</summary>
		public IdleQueue(CrashDoc hostDoc)
		{
			this._hostDoc = hostDoc;
			_idleQueue = new ConcurrentQueue<IdleAction>();
		}

		/// <summary>Adds an Action to the Queue</summary>
		public async Task AddActionAsync(IdleAction action)
		{
			await Task.Run(() => _idleQueue.Enqueue(action));
		}

		/// <summary>Attempts to run the next Action</summary>
		public void RunNextAction()
		{
			if (_idleQueue.IsEmpty) return;

			if (!_idleQueue.TryDequeue(out var action)) return;
			action?.Invoke();

			if (_idleQueue.IsEmpty)
			{
				OnCompletedQueue?.Invoke(this, null);
			}
		}

		/// <summary>The number of items in the Queue</summary>
		public int Count => _idleQueue.Count;

		/// <summary>GetEnumerator</summary>
		public IEnumerator<IdleAction> GetEnumerator() => _idleQueue.GetEnumerator();

		/// <summary>GetEnumerator</summary>
		IEnumerator IEnumerable.GetEnumerator() => _idleQueue.GetEnumerator();

		/// <summary>Fires when the queue has finished parsing more than 1 item.</summary>
		public event EventHandler OnCompletedQueue;

	}

}
