using System.Collections;
using Crash.Common.Document;

namespace Crash.Events
{

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

		public bool TryDequeue(out IdleAction action)
			=> idleQueue.TryDequeue(out action);

		public int Count => idleQueue.Count;

        public IEnumerator<IdleAction> GetEnumerator() => idleQueue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => idleQueue.GetEnumerator();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

}
