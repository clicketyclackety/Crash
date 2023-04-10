namespace Crash.Handlers.InternalEvents
{

	public sealed class CrashSelectionEventArgs : EventArgs
	{

		public readonly IEnumerable<CrashObject> CrashObjects;
		public readonly bool Selected;
		public readonly bool All;

		public CrashSelectionEventArgs(bool selected,
								IEnumerable<CrashObject> crashObjects)
		{
			CrashObjects = crashObjects;
			Selected = selected;
			All = false;
		}

		public CrashSelectionEventArgs(bool selected = false)
		{
			All = true;
			Selected = selected;
		}

	}

}
