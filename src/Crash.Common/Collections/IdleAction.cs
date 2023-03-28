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
		readonly Action<CrashEventArgs> _action;
		readonly CrashEventArgs _args;

		/// <summary>True after the action has been invoked.s</summary>
		public bool Invoked { get; private set; } = false;

		/// <summary>Creates an Idle Action</summary>
		public IdleAction(Action<CrashEventArgs> action, CrashEventArgs args)
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
