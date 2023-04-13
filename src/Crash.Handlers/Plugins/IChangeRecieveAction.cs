using Crash.Common.Document;

namespace Crash.Handlers.Plugins
{
	// TODO :
	// How do we handle the fact we have Add/Remove/Transform endpoints?
	// This only handles the initial creation
	/// <summary>Handles recieved changes from the Server</summary>
	public interface IChangeRecieveAction
	{

		/// <summary>The Action this ICreateAction responds to</summary>
		ChangeAction Action { get; }

		/// <summary>Deserializes a Server Sent Change</summary>
		public Task OnRecieveAsync(CrashDoc crashDoc, Change recievedChange);

	}

}
