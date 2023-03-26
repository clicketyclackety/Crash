using Crash.Common.Document;

using Rhino.Display;
using Rhino.Geometry;

namespace Crash.Handlers.Plugins.Schemas
{

	public abstract class ChangeConverter<TChange> where TChange : IChange
	{
		private Dictionary<int, Func<CrashDoc, TChange, Task>> actionSwitch;

		public readonly Type ChangeType;

		protected ChangeConverter()
		{
			ChangeType = typeof(TChange);
		}

		// Register Change
		// Register Change Add to Doc
		// Register Change Remove from Doc
		// Register Change Draw in Pipeline

		// Register ChangeActions scenarios when recieved from Server
		// i.e 1, 2, 3, 4
		protected void RegisterChangeAction(int changeAction,
											ICrashEvent crashEvent)
		{
			actionSwitch.Add(changeAction, crashEvent);
		}

		public abstract TChange Convert(object @object);

		public abstract bool ShouldConvert(object @object);

		public abstract void DrawChange(TChange change, DrawEventArgs draw,
										DisplayMaterial material);

		public virtual bool ShouldDraw(TChange change) => true;

		public abstract BoundingBox GetBounds(TChange change);

	}

	class Example
	{
		/// PERFECT /// IDEAL CONVERT FUNCTION
		private static bool TryGet(object @object, out IChange change)
		{
			IEnumerable<ChangeConverter<IChange>> changeConverters = null;
			foreach (var changeConverter in changeConverters)
			{
				if (!changeConverter.ShouldConvert(@object)) continue;
				change = changeConverter.Convert(@object);
				return true;
			}

			change = null;
			return false;
		}

		/// PERFECT /// IDEAL DRAW FUNCTION
		private static void TryDraw(IChange change)
		{
			IEnumerable<ChangeConverter<IChange>> changeConverters = null;
			foreach (var changeConverter in changeConverters)
			{
				if (!changeConverter.ShouldDraw(change)) continue;
				changeConverter.DrawChange(change, null, null);
				return;
			}
		}

		private static async Task TryRecieveAction(IChange change)
		{
			CrashDoc crashDoc = null;
			Dictionary<int, List<ICrashEvent>> actions = null;

			if (!actions.TryGetValue(change.Action, out List<ICrashEvent> actionEvents)) return;
			foreach (var actionEvent in actionEvents)
			{
				await actionEvent.Recieve(crashDoc, change);
			}
		}

		private static async Task<bool> ActionHappened(ChangeAction action, object args, out IChange change)
		{
			CrashDoc crashDoc = null;
			Dictionary<int, List<ICrashEvent>> actions = null;

			if (!actions.TryGetValue((int)action, out List<ICrashEvent> actionEvents))
				return false;

			TryGet(args, out IChange change)

			foreach (var actionEvent in actionEvents)
			{

				TryGet(args, out IChange change)
				change = await actionEvent.Send(crashDoc);
				return true;
			}

			change = null;
			return false;
		}

	}

}
