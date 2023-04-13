using Crash.Common.Document;

namespace Crash.Common.Events
{

	/// <summary>The Crash Event Args</summary>
	public class CrashEventArgs : EventArgs
	{
		/// <summary>The Crash Doc of these Args</summary>
		public readonly CrashDoc CrashDoc;

		/// <summary>Default Constructor</summary>
		public CrashEventArgs(CrashDoc crashDoc)
		{
			CrashDoc = crashDoc;
		}


	}

}
