using Crash.Common.Document;

using Rhino;

namespace Crash.Handlers.Plugins
{

	/// <summary>A wrapper for Crash Args</summary>
	public class CreateRecieveArgs : EventArgs
	{

		/// <summary>The Action</summary>
		public readonly ChangeAction Action;
		/// <summary>The EventArgs, often wrapped Rhino Args</summary>
		public readonly EventArgs Args;
		/// <summary>The current Crash Document</summary>
		public readonly CrashDoc Doc;

		/// <summary>Internal Constructor</summary>
		public CreateRecieveArgs(ChangeAction action, EventArgs args, CrashDoc doc)
		{
			Action = action;
			Args = args ?? throw new ArgumentNullException(nameof(EventArgs));
			Doc = doc ?? throw new ArgumentNullException(nameof(CrashDoc));
		}

		/// <summary>Constructor with Rhino Doc</summary>
		public CreateRecieveArgs(ChangeAction action, EventArgs args, RhinoDoc doc)
			: this(action, args, CrashDocRegistry.GetRelatedDocument(doc))
		{
		}

	}

}
