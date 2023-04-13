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

		/// <summary>True if successfully invoked</summary>
		public bool Invoked { get; private set; }

		/// <summary>Constructs an Idle Action</summary>
		/// <param name="action">The Action to be called on Idle</param>
		/// <param name="args">The Args to be passed into the Action</param>
		/// <exception cref="ArgumentNullException">If any inputs are null</exception>
		public IdleAction(Action<IdleArgs> action, IdleArgs args)
		{
			_action = action ?? throw new ArgumentNullException($"{nameof(action)} is null");
			_args = args ?? throw new ArgumentNullException($"{nameof(args)} is null");
		}

		/// <summary>Invokes the Action (If it hasn't already been invoked)</summary>
		public void Invoke()
		{
			if (Invoked) return;
			_action(_args);
			Invoked = true;
		}

		/// <inheritdoc/>
		public void Dispose()
		{

		}

	}

}
