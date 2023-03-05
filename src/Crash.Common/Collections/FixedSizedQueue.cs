using System.Collections;

namespace Crash.Common.Collections
{

    public sealed class FixedSizedQueue<T> : IReadOnlyCollection<T>
    {
        private readonly Queue<T> _queue;

        public readonly int Size;

        public FixedSizedQueue(int size)
        {
            Size = size;
            _queue = new Queue<T>();
        }

        public void Enqueue(T item)
        {
            if (_queue.Count >= Size)
            {
                _queue.Dequeue();
            }

            _queue.Enqueue(item);
        }

        public int Count => _queue.Count;

        public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator();

    }

}
