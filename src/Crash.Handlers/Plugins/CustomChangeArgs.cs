using Crash.Common.Document;

using Rhino.Display;
using Rhino.Geometry;

namespace Crash.Handlers.Plugins
{
	public sealed class CustomChangeArgs<TChange> where TChange : IChange
	{
		public Action<TChange, CrashDoc> AddAction;
		public Action<TChange, CrashDoc> RemoveAction;
		public Action<TChange, DrawEventArgs, DisplayMaterial> DrawArgs;
		public Func<TChange, BoundingBox> GetBoundingBox;
	}

}
