using Crash.Common.Document;

namespace Crash.Handlers.Plugins
{

	public abstract class ChangeSchema<TChange> where TChange : IChange
	{

		protected ChangeSchema() { }

		// Register Change
		// Register Change Add to Doc
		// Register Change Remove from Doc
		// Register Change Draw in Pipeline

		// Register ChangeActions scenarios when recieved from Server
		// i.e 1, 2, 3, 4
		protected void RegisterChangeAction(int changeAction, Func<CrashDoc, TChange, Task> action)
		{

		}

		protected void RegisterCustomChange(CustomChangeArgs<TChange> customChangeArgs)
		{

		}

		internal void RegisterSchema(CrashDoc crashDoc)
		{

		}

	}

}
