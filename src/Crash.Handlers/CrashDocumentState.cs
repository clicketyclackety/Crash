using Crash.Common.Document;
using Crash.Handlers.Plugins;

using Rhino;

namespace Crash.Handlers
{
	// TODO : Make internal
	// TODO : Is this needed?
	public sealed class CrashDocumentState : IDisposable
	{

		public CrashDoc Document;
		public EventDispatcher Dispatcher;

		internal CrashDocumentState(CrashDoc document)
		{
			Document = document;
			RhinoApp.Idle += CallIdle;
		}

		public override int GetHashCode() => Document.GetHashCode();

		private void CallIdle(object sender, EventArgs e)
		{
			Document.Queue.RunNextAction();
		}

		private void Queue_OnCompletedQueue(object sender, EventArgs e)
		{
			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(Document);
			rhinoDoc?.Views?.Redraw();
		}

		public void Dispose()
		{
			// TODO : What if there are still things in the Queue?
			RhinoApp.Idle -= CallIdle;
		}
	}
}
