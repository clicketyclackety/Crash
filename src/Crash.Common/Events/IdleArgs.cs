using Crash.Common.Document;

namespace Crash.Common.Events
{

	public class IdleArgs : EventArgs
	{
		public CrashDoc CrashDoc { get; set; }


		public IdleArgs(CrashDoc _crashDoc)
		{
			CrashDoc = _crashDoc;
		}
	}
}
