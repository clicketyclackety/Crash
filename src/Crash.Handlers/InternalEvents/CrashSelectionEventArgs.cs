namespace Crash.Handlers.InternalEvents
{

	/// <summary>Wrapss the RhinoSelection and Deselection Events</summary>
	public sealed class CrashSelectionEventArgs : EventArgs
	{

		/// <summary>Related Event Objets</summary>
		public readonly IEnumerable<CrashObject> CrashObjects;
		/// <summary>Was this a Selection Event?</summary>
		public readonly bool Selected;
		/// <summary>Used only on Deselect All Event</summary>
		public readonly bool DeselectAll;

		/// <summary>Singular Selection/Deselection Event Constructor</summary>
		public CrashSelectionEventArgs(bool selected,
								IEnumerable<CrashObject> crashObjects)
		{
			CrashObjects = crashObjects;
			Selected = selected;
			DeselectAll = false;
		}

		/// <summary>Deselect All Event Constructor</summary>
		public CrashSelectionEventArgs(bool selected = false)
			: this(selected, Enumerable.Empty<CrashObject>())
		{
			DeselectAll = true;
		}

	}

}
