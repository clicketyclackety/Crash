using Rhino.Display;
using Rhino.Geometry;

namespace Crash.Handlers.Plugins
{
	public sealed class CustomChangeArgs
	{
		public readonly Type ChangeType;

		public Action<IChange, DrawEventArgs, DisplayMaterial> DrawArgs { get; private set; }
		public Func<IChange, BoundingBox> GetBoundingBox { get; private set; }

		private CustomChangeArgs(Type changeType)
		{
			this.ChangeType = changeType;
		}

		public static CustomChangeArgs Create<TChange>(
			Action<IChange, DrawEventArgs, DisplayMaterial> drawChange = null,
			Func<IChange, BoundingBox> getBoundingBox = null
			) where TChange : IChange
		{
			return new CustomChangeArgs(typeof(TChange))
			{
				DrawArgs = drawChange,
				GetBoundingBox = getBoundingBox
			};
		}

	}

}
