﻿using Crash.Common.Document;

namespace Crash.Common.Events
{

	public class CrashEventArgs : EventArgs
	{
		public CrashDoc CrashDoc { get; set; }


		public CrashEventArgs(CrashDoc _crashDoc)
		{
			CrashDoc = _crashDoc;
		}

	}

}
