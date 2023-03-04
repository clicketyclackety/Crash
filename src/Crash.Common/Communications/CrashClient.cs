using Crash.Common.Document;
using Crash.Communications;

using Microsoft.AspNetCore.SignalR.Client;

namespace Crash.Client
{

	/// <summary>
	/// Crash client class
	/// </summary>
	public class CrashClient
	{
		#region consts
		const string ADD = "Add";
		const string DELETE = "Delete";
		const string DONE = "Done";
		const string UPDATE = "Update";
		const string SELECT = "Select";
		const string UNSELECT = "Unselect";
		const string INITIALIZE = "Initialize";
		const string CAMERACHANGE = "CameraChange";

		public const string DefaultURL = "http://localhost";
		#endregion

		HubConnection _connection;
		string _user;
		CrashDoc _crashDoc;

		public bool IsConnected => _connection.State != HubConnectionState.Disconnected;
		public HubConnectionState State => _connection.State;

		/// <summary>
		/// Closed event
		/// </summary>
		public event Func<Exception, Task> Closed
		{
			add => _connection.Closed += value;
			remove => _connection.Closed -= value;
		}

		public event Action<string, Change> OnAdd;
		public event Action<string, Guid> OnDelete;
		public event Action<string, Guid, Change> OnUpdate;
		public event Action<string> OnDone;
		public event Action<string, Guid> OnSelect;
		public event Action<string, Guid> OnUnselect;
		public event Action<Change[]> OnInitialize;
		public event Action<string, Change> OnCameraChange;

		/// <summary>
		/// Stop async task
		/// </summary>
		/// <returns></returns>
		public Task StopAsync() => _connection.StopAsync();

		/// <summary>
		/// Crash client constructor
		/// </summary>
		/// <param name="userName">user name</param>
		/// <param name="url">url</param>
		public CrashClient(CrashDoc crashDoc, string userName, Uri url)
		{
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentException("Username cannot be empty or null");
			}
			if (null == url)
			{
				throw new UriFormatException("URL Cannot be null");
			}
			if (!url.AbsoluteUri.Contains("/Crash"))
			{
				throw new UriFormatException("URL must end in /Crash to connect!");
			}

			_crashDoc = crashDoc;
			_user = userName;
			_connection = getHubConnection(url);
			RegisterConnections();
		}

		internal static HubConnection getHubConnection(Uri url)
		{
			return new HubConnectionBuilder()
			   .WithUrl(url)
			   .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromSeconds(10) })
			   .Build();
		}

		internal void RegisterConnections()
		{
			// How to test?
			// Does it need to be AddAsync now?
			_connection.On<string, Change>(ADD, OnAddInvoke);
			_connection.On<string, Guid>(DELETE, (user, id) => OnDelete?.Invoke(user, id));
			_connection.On<string, Guid, Change>(UPDATE, (user, id, Change) => OnUpdate?.Invoke(user, id, Change));
			_connection.On<string>(DONE, (user) => OnDone?.Invoke(user));
			_connection.On<string, Guid>(SELECT, (user, id) => OnSelect?.Invoke(user, id));
			_connection.On<string, Guid>(UNSELECT, (user, id) => OnUnselect?.Invoke(user, id));
			_connection.On<Change[]>(INITIALIZE, (Changes) => OnInitialize?.Invoke(Changes));
			_connection.On<string, Change>(CAMERACHANGE, (user, Change) => OnCameraChange?.Invoke(user, Change));

			_connection.Reconnected += ConnectionReconnectedAsync;
			_connection.Closed += ConnectionClosedAsync;
			_connection.Reconnecting += ConnectionReconnectingAsync;
		}

		private void OnAddInvoke(string user, Change change)
		{
			OnAdd?.Invoke(user, change);
		}

		// TODO : Shouldn't be static.
		public async Task StartLocalClient(Action<IEnumerable<Change>> OnInit)
		{
			if (null == _crashDoc)
			{
				throw new NullReferenceException("CrashDoc cannot be null!");
			}

			string userName = _crashDoc?.Users?.CurrentUser.Name;
			if (string.IsNullOrEmpty(userName))
			{
				throw new Exception("A User has not been assigned!");
			}

			this.OnInitialize += OnInit;

			// TODO : Check for successful connection
			await this.StartAsync();
		}

		public static async Task CloseLocalServer(CrashDoc crashDoc)
		{
			CrashServer? server = crashDoc?.LocalServer;
			if (null == server) return;

			server?.Stop();
			server?.Dispose();
		}

		private Task ConnectionReconnectingAsync(Exception? arg)
		{
			Console.WriteLine(arg);
			return Task.CompletedTask;
		}

		private Task ConnectionClosedAsync(Exception? arg)
		{
			Console.WriteLine(arg);
			return Task.CompletedTask;
		}

		private Task ConnectionReconnectedAsync(string? arg)
		{
			Console.WriteLine(arg);
			return Task.CompletedTask;
		}

		/// <summary>
		/// Update task
		/// </summary>
		/// <param name="id">id</param>
		/// <param name="Change">Change</param>
		/// <returns></returns>
		public async Task UpdateAsync(Guid id, Change Change)
		{
			await _connection.InvokeAsync(UPDATE, _user, id, Change);
		}

		/// <summary>
		/// Delete task
		/// </summary>
		/// <param name="id">id</param>
		/// <returns>returns task</returns>
		public async Task DeleteAsync(Guid id)
		{
			await _connection.InvokeAsync(DELETE, _user, id);
		}

		/// <summary>Adds a change to database </summary>
		public async Task AddAsync(Change Change)
		{
			await _connection.InvokeAsync(ADD, _user, Change);
		}

		/// <summary>Done</summary>
		public async Task DoneAsync()
		{
			await _connection.InvokeAsync(DONE, _user);
		}

		/// <summary>Select event</summary>
		public async Task SelectAsync(Guid id)
		{
			await _connection.InvokeAsync(SELECT, _user, id);
		}

		/// <summary>
		/// Unselect event
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task UnselectAsync(Guid id)
		{
			await _connection.InvokeAsync(UNSELECT, _user, id);
		}

		/// <summary>
		/// CameraChange event
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task CameraChangeAsync(Change Change)
		{
			await _connection.InvokeAsync(CAMERACHANGE, _user, Change);
		}

		/// <summary>
		/// Start the async connection
		/// </summary>
		/// <returns></returns>
		public Task StartAsync() => _connection.StartAsync();

	}

}
