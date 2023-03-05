using System;

using Crash.Common.Document;
using Crash.Common.Events;

namespace Crash.Events.Args
{

	internal sealed class DeleteArgs : CrashEventArgs
	{

		internal readonly Guid ChangeId;

		public DeleteArgs(CrashDoc crashDoc, Guid changeId)
			: base(crashDoc)
		{
			ChangeId = changeId;
		}

	}

}
