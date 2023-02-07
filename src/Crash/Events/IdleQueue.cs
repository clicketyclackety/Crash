namespace Crash.Events
{

    public sealed class IdleQueue : IDisposable
	{
		private ConcurrentQueue<IdleAction> idleQueue;

		internal IdleQueue()
		{
			idleQueue = new ConcurrentQueue<IdleAction>();
            RhinoApp.Idle += CallIdle;
		}

        private void CallIdle(object sender, EventArgs e)
        {
			idleQueue.TryDequeue(out IdleAction action);
			action.Invoke();
        }

        internal bool RunQueue()
		{
			if (!idleQueue.TryDequeue(out IdleAction action)) return false;

			action.Invoke();

			return idleQueue.Count == 0;
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
