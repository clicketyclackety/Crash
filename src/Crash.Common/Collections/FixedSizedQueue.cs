using System.Collections;

namespace Crash.Common.Collections
{

	/// <summary>A Queue of a predetermined size</summary>
	public sealed class FixedSizedQueue<T> : IReadOnlyCollection<T>
	{
		private readonly Queue<T> _queue;

		public readonly int Size;

		/// <summary></summary>
		public FixedSizedQueue(int size)
		{
			Size = size;
			_queue = new Queue<T>();
		}

		/// <summary>Adds an item to the Queue, removing the first item if adding would put it oversize.</summary>
		public void Enqueue(T item)
		{
			if (_queue.Count >= Size)
			{
				_queue.Dequeue();
			}

			_queue.Enqueue(item);
		}

		/// <summary></summary>
		public int Count => _queue.Count;

		/// <summary></summary>
		public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();

		/// <summary></summary>
		IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator();

	}

}
