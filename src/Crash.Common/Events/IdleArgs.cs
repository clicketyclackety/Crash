using Crash.Common.Document;

namespace Crash.Common.Events
{

	public class IdleArgs : EventArgs
	{

		public readonly CrashDoc Doc;
		public readonly IChange Change;

		public IdleArgs(CrashDoc crashDoc, IChange change)
		{
			Doc = crashDoc;
			Change = change;
		}

	}

}
