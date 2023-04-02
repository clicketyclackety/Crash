﻿using Crash.Common.Events;

namespace Crash.Events
{

	/// <summary>
	/// Idle Actions are called in a FIFO order by the IdleQueue.
	/// Inheriting from this is fine.
	/// Do not override Invoke however.
	/// </summary>
	public class IdleAction : IDisposable
	{

		Action<IdleArgs> _action;

		IdleArgs _args;

		public bool Invoked { get; private set; }

		public IdleAction(Action<IdleArgs> action, IdleArgs args)
		{
			_action = action;
			_args = args;
		}

		public void Invoke()
		{
			_action(_args);
			Invoked = true;
		}

		public void Dispose()
		{

		}

	}

}
