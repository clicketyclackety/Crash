namespace Crash.Changes.Extensions
{
	public static class UpdateAction
	{

		public static void AddAction(this IChange change, ChangeAction action)
		{
			int actionAsInt = change.Action;
			actionAsInt |= (int)action;

			change.Action = actionAsInt;
		}

		public static void RemoveAction(this IChange change, ChangeAction action)
		{
			int actionAsInt = change.Action;
			actionAsInt &= ~(int)action;

			change.Action = actionAsInt;
		}

		public static void ToggleAction(this IChange change, ChangeAction action)
		{
			int actionAsInt = change.Action;
			actionAsInt ^= (int)action;

			change.Action = actionAsInt;
		}

	}
}
