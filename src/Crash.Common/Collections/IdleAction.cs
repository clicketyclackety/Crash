using Crash.Common.Events;

namespace Crash.Events
{
	public sealed class IdleAction
	{

		Action<CrashEventArgs> _action;

		CrashEventArgs _args;

		public IdleAction(Action<CrashEventArgs> action, CrashEventArgs args)
		{
			_action = action;
			_args = args;
		}

		public void Invoke()
		{
			_action(_args);
		}

	}

}
