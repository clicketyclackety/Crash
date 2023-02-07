namespace Crash.Events
{
    public sealed class IdleAction
	{

		Action<EventArgs> _action;

		EventArgs _args;

		public IdleAction(Action<EventArgs> action, EventArgs args)
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
