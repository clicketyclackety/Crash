using Crash.Common.Document;

namespace Crash.Handlers.Plugins.Schemas
{

	public interface ICrashEvent
	{
		public ChangeAction ChangeAction { get; }

		/// <summary>Send a new Change to the server</summary>
		Func<CrashDoc, object, Task<IChange>> Send { get; }

		/// <summary>Recieve a Change from the Server</summary>
		Func<CrashDoc, IChange, Task> Recieve { get; }
	}

	// Should we pair Actions
	public sealed class CrashEvent : ICrashEvent
	{
		public Func<CrashDoc, Task<IChange>> Send { get; set; }
		public Func<CrashDoc, IChange, Task> Recieve { get; set; }

		public CrashEvent(ChangeAction changeAction,
						  Func<CrashDoc, Task<IChange>> send,
						  Func<CrashDoc, IChange, Task> recieve)
		{
			ChangeAction = changeAction;
			Send = send;
			Recieve = recieve;
		}

	}


























	// Should we pair Actions
	public sealed class CrashAddEvent : ICrashEvent
	{
		public Func<CrashDoc, Task<IChange>> Send { get; set; }
		public Func<CrashDoc, IChange, Task> Recieve { get; set; }
	}

	// Should we pair Actions
	public sealed class CrashRemoveEvent : ICrashEvent
	{
		public Func<CrashDoc, Task<IChange>> Send { get; set; }
		public Func<CrashDoc, IChange, Task> Recieve { get; set; }
	}

	// Should we pair Actions
	public sealed class CrashLockEvent : ICrashEvent
	{
		public Func<CrashDoc, IChange, Task> addFromServer;
		public Func<CrashDoc, Task<IChange>> addToServer;
	}

	// Should we pair Actions
	public sealed class CrashUnlockEvent : ICrashEvent
	{
		public Func<CrashDoc, IChange, Task> addFromServer;
		public Func<CrashDoc, Task<IChange>> addToServer;
	}

	public sealed class CrashTransformEvent : ICrashEvent
	{
		public Func<CrashDoc, IChange, Task> addFromServer;
		public Func<CrashDoc, Task<IChange>> addToServer;
	}

	public sealed class CrashCameraEvent : ICrashEvent
	{
		public Func<CrashDoc, IChange, Task> addFromServer;
		public Func<CrashDoc, Task<IChange>> addToServer;
	}

}
