using Crash.Common.Events;

namespace Crash.Events
{

	/// <summary>
	/// Idle Actions are called in a FIFO order by the IdleQueue.
	/// Inheriting from this is fine.
	/// Do not override Invoke however.
	/// </summary>
	public class IdleAction : IDisposable
	{
		readonly Action<IdleArgs> _action;
		readonly IdleArgs _args;

		public bool Invoked { get; private set; }

		public IdleAction(Action<IdleArgs> action, IdleArgs args)
		{
			_action = action ?? throw new ArgumentNullException($"{nameof(action)} is null");
			_args = args ?? throw new ArgumentNullException($"{nameof(args)} is null");
		}

		/// <summary>Invokes the Action (if it hasn't already been invoked)</summary>
		public void Invoke()
		{
			if (Invoked) return;
			_action(_args);
			Invoked = true;
		}

		public void Dispose()
		{

		}

	}

}
