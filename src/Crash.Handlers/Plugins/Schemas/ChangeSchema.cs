using Crash.Common.Document;

namespace Crash.Handlers.Plugins.Schemas
{

	public abstract class ChangeSchema
	{
		private Dictionary<int, Func<CrashDoc, IChange, Task>> actionSwitch;

		public readonly Type ChangeType;
		public readonly CustomChangeArgs ChangeArgs;

		protected ChangeSchema(CustomChangeArgs customChangeArgs)
		{
			actionSwitch = new Dictionary<int, Func<CrashDoc, IChange, Task>>();

			ChangeType = customChangeArgs.ChangeType;
		}

		// Register Change
		// Register Change Add to Doc
		// Register Change Remove from Doc
		// Register Change Draw in Pipeline

		// Register ChangeActions scenarios when recieved from Server
		// i.e 1, 2, 3, 4
		protected void RegisterChangeAction(int changeAction, Func<CrashDoc, IChange, Task> action)
		{
			actionSwitch.Add(changeAction, action);
		}

	}

}
