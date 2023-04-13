using Crash.Common.Document;

using Rhino;

namespace Crash.Handlers.Plugins
{

	/// <summary></summary>
	public class CreateRecieveArgs : EventArgs
	{

		/// <summary></summary>
		public readonly ChangeAction Action;
		/// <summary></summary>
		public readonly EventArgs Args;
		/// <summary></summary>
		public readonly CrashDoc Doc;

		/// <summary></summary>
		public CreateRecieveArgs(ChangeAction action, EventArgs args, CrashDoc doc)
		{
			Action = action;
			Args = args ?? throw new ArgumentNullException(nameof(EventArgs));
			Doc = doc ?? throw new ArgumentNullException(nameof(CrashDoc));
		}

		/// <summary></summary>
		public CreateRecieveArgs(ChangeAction action, EventArgs args, RhinoDoc doc)
			: this(action, args, CrashDocRegistry.GetRelatedDocument(doc))
		{
		}

	}

}
