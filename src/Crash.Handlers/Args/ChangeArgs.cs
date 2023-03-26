using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Events;

namespace Crash.Handlers.Args
{
	public sealed class ChangeArgs : CrashEventArgs
	{

		public readonly GeometryChange Change;

		public ChangeArgs(CrashDoc _crashDoc, GeometryChange change) : base(_crashDoc)
		{
			Change = change;
		}

	}
}
