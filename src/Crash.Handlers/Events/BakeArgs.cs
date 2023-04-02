using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Events;

using Rhino.Geometry;

namespace Crash.Events.Args
{

	internal sealed class BakeArgs : IdleArgs
	{
		internal GeometryBase? Geometry => Change.Geometry;
		internal new readonly GeometryChange Change;


		public BakeArgs(CrashDoc crashDoc, GeometryChange change)
			: base(crashDoc, change)
		{
			Change = change;
		}

	}

}
