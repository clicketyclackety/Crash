namespace Crash.Changes.Extensions
{

	public static class Utils
	{

		public static bool IsTemporary(this IChange change)
			=> change.GetChangeAction().HasFlag(ChangeAction.Temporary);

		public static ChangeAction GetChangeAction(this IChange change)
			=> (ChangeAction)change.Action;

		public static bool HasFlag<T>(this IChange change, T flag) where T : Enum
			=> change.GetChangeAction().HasFlag(flag);

	}

}
