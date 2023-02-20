using Microsoft.AspNetCore.SignalR.Client;

namespace Crash.Client
{
	/// <summary>
	/// Crash client class
	/// </summary>
	public sealed class CrashClient
	{
		HubConnection _connection;
		string _user;

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
		public CrashClient(string userName, Uri url)
		{
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentException("Username cannot be empty or null");
			}
			if (null == url)
			{
				throw new UriFormatException("URL Cannot be null");
			}

			_user = userName;
			_connection = new HubConnectionBuilder()
				.WithUrl(url)
				.WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromSeconds(10) })
				.Build();

			_connection.On<string, Change>("Add", (user, Change) => OnAdd?.Invoke(user, Change));
			_connection.On<string, Guid>("Delete", (user, id) => OnDelete?.Invoke(user, id));
			_connection.On<string, Guid, Change>("Update", (user, id, Change) => OnUpdate?.Invoke(user, id, Change));
			_connection.On<string>("Done", (user) => OnDone?.Invoke(user));
			_connection.On<string, Guid>("Select", (user, id) => OnSelect?.Invoke(user, id));
			_connection.On<string, Guid>("Unselect", (user, id) => OnUnselect?.Invoke(user, id));
			_connection.On<Change[]>("Initialize", (Changes) => OnInitialize?.Invoke(Changes));
			_connection.On<string, Change>("CameraChange", (user, Change) => OnCameraChange?.Invoke(user, Change));

			_connection.Closed += ConnectionClosedAsync;
			_connection.Reconnecting += ConnectionReconnectingAsync;
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

		/// <summary>
		/// Update task
		/// </summary>
		/// <param name="id">id</param>
		/// <param name="Change">Change</param>
		/// <returns></returns>
		public async Task UpdateAsync(Guid id, Change Change)
		{
			await _connection.InvokeAsync("Update", _user, id, Change);
		}

		/// <summary>
		/// Delete task
		/// </summary>
		/// <param name="id">id</param>
		/// <returns>returns task</returns>
		public async Task DeleteAsync(Guid id)
		{
			await _connection.InvokeAsync("Delete", _user, id);
		}

		/// <summary>
		/// Add to database
		/// </summary>
		/// <param name="Change">Change</param>
		/// <returns>task</returns>
		public async Task AddAsync(Change Change)
		{
			await _connection.InvokeAsync("Add", _user, Change);
		}

		/// <summary>Done</summary>
		public async Task DoneAsync()
		{
			await _connection.InvokeAsync("Done", _user);
		}

		/// <summary>Select event</summary>
		public async Task SelectAsync(Guid id)
		{
			await _connection.InvokeAsync("Select", _user, id);
		}

		/// <summary>
		/// Unselect event
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task UnselectAsync(Guid id)
		{
			await _connection.InvokeAsync("Unselect", _user, id);
		}

		/// <summary>
		/// CameraChange event
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task CameraChangeAsync(Change Change)
		{
			await _connection.InvokeAsync("CameraChange", _user, Change);
		}

		/// <summary>
		/// Start the async connection
		/// </summary>
		/// <returns></returns>
		public Task StartAsync() => _connection.StartAsync();

	}

}
