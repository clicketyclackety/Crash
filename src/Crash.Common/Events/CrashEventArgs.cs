using Crash.Common.Document;

namespace Crash.Common.Events
{

	public class CrashEventArgs : EventArgs
	{

		public readonly CrashDoc CrashDoc;

		public CrashEventArgs(CrashDoc crashDoc)
		{
			CrashDoc = crashDoc;
		}


	}

}
