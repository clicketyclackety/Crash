﻿using System.Collections;

namespace Crash.Common.Collections
{

	/// <summary>A Queue of a predetermined size</summary>
	public sealed class FixedSizedQueue<T> : IReadOnlyCollection<T>
	{
		private readonly Queue<T> _idleQueue;

		public readonly int Size;

		/// <summary></summary>
		public FixedSizedQueue(int size)
		{
			Size = size;
			_idleQueue = new Queue<T>();
		}

		/// <summary>Adds an item to the Queue, removing the first item if adding would put it oversize.</summary>
		public void Enqueue(T item)
		{
			if (_idleQueue.Count >= Size)
			{
				_idleQueue.Dequeue();
			}

			_idleQueue.Enqueue(item);
		}

		/// <summary></summary>
		public int Count => _idleQueue.Count;

		/// <summary></summary>
		public IEnumerator<T> GetEnumerator() => _idleQueue.GetEnumerator();

		/// <summary></summary>
		IEnumerator IEnumerable.GetEnumerator() => _idleQueue.GetEnumerator();

	}

}
