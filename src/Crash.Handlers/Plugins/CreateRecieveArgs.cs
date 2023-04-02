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
			Args = args;
			Doc = doc;
		}

		/// <summary></summary>
		public CreateRecieveArgs(ChangeAction action, EventArgs args, RhinoDoc doc)
		{
			Action = action;
			Args = args;
			Doc = CrashDocRegistry.GetRelatedDocument(doc);
		}

	}

}
