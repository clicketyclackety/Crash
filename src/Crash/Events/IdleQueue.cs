using Crash.Document;

namespace Crash.Events
{

    public sealed class IdleQueue : IDisposable
	{
		private ConcurrentQueue<IdleAction> idleQueue;

        private CrashDoc hostDoc;

        internal IdleQueue(CrashDoc hostDoc)
		{
			this.hostDoc = hostDoc;
			idleQueue = new ConcurrentQueue<IdleAction>();
            RhinoApp.Idle += CallIdle;
		}

        private void CallIdle(object sender, EventArgs e)
        {
			if (!idleQueue.TryDequeue(out IdleAction action)) return;
			
			action.Invoke();

			// Only runs after a queue is finished
            if (idleQueue.Count == 0)
			{
                hostDoc.Redraw();
			}
        }

		internal void AddAction(IdleAction action)
		{
			idleQueue.Enqueue(action);
		}

        public void Dispose()
        {
            // TODO : What if there are still things in the Queue?
			RhinoApp.Idle -= CallIdle;
        }

    }

}
