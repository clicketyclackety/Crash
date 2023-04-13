using System.Diagnostics;

using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Common.Exceptions;
using Crash.Common.Logging;

using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Crash.Client
{

	/// <summary>
	/// Crash client class
	/// </summary>
	public sealed class CrashClient
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

		// TODO : Move to https
		public const string DefaultURL = "http://localhost";
		#endregion

		readonly HubConnection _connection;
		readonly string _user;
		readonly CrashDoc _crashDoc;

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
			_connection = GetHubConnection(url);
			RegisterConnections();
		}

		internal static HubConnection GetHubConnection(Uri url) => new HubConnectionBuilder()
			   .WithUrl(url).AddJsonProtocol()
			   .AddJsonProtocol((opts) => JsonOptions())
			   // .ConfigureLogging(LoggingConfigurer)
			   .WithAutomaticReconnect(new[] { TimeSpan.FromMilliseconds(10),
											   TimeSpan.FromMilliseconds(100),
											   TimeSpan.FromSeconds(1),
											   TimeSpan.FromSeconds(10) })
			   .Build();

		internal static JsonHubProtocolOptions JsonOptions() => new JsonHubProtocolOptions()
		{
			PayloadSerializerOptions = new System.Text.Json.JsonSerializerOptions()
			{
				IgnoreReadOnlyFields = true,
				IgnoreReadOnlyProperties = true,
				NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
			}
		};

		private static void LoggingConfigurer(ILoggingBuilder loggingBuilder)
		{
			LogLevel logLevel = Debugger.IsAttached ? LogLevel.Trace : LogLevel.Information;
			loggingBuilder.SetMinimumLevel(logLevel);
			var loggingProvider = new CrashLoggerProvider();
			loggingBuilder.AddProvider(loggingProvider);
		}

		internal void RegisterConnections()
		{
			_connection.On<string, Change>(ADD, (user, change) => OnAdd?.Invoke(user, change));
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

		public async Task StartLocalClientAsync()
		{
			if (null == _crashDoc)
			{
				throw new NullReferenceException("CrashDoc cannot be null!");
			}

			string? userName = _crashDoc?.Users?.CurrentUser.Name;
			if (string.IsNullOrEmpty(userName))
			{
				throw new Exception("A User has not been assigned!");
			}

			this.OnInitialize += Init;

			// TODO : Check for successful connection
			await this.StartAsync();
		}

		// This isn't calling, and needs to call the Event Dispatcher
		private void Init(IEnumerable<Change> changes)
		{
			OnInit?.Invoke(this, new CrashInitArgs(_crashDoc, changes));
		}

		public static void CloseLocalServer(CrashDoc crashDoc)
		{
			crashDoc?.LocalServer?.Stop();
			crashDoc?.LocalServer?.Dispose();
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
			int changeLength = Change.Payload.Length;
			if (changeLength >= ushort.MaxValue)
			{
				throw new OversizedChangeException($"Change is over maximum size. {changeLength}/{ushort.MaxValue}");
			}

			CrashLogger.Logger.LogInformation($"Change {Change.Id} size is {changeLength}");

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
		private Task StartAsync() => _connection.StartAsync();

		public static event EventHandler<CrashInitArgs> OnInit;

		public sealed class CrashInitArgs : CrashEventArgs
		{
			public readonly IEnumerable<Change> Changes;

			public CrashInitArgs(CrashDoc crashDoc, IEnumerable<Change> changes)
				: base(crashDoc)
			{
				Changes = changes;
			}
		}

	}

}
