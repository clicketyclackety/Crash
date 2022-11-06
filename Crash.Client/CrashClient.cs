using Microsoft.AspNetCore.SignalR.Client;
using SpeckLib;
using System;
using System.Threading.Tasks;

namespace Crash
{
    /// <summary>
    /// Crash client class
    /// </summary>
    public class CrashClient
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

        public event Action<string, Speck> OnAdd;
        public event Action<string, Guid> OnDelete;
        public event Action<string, Guid, Speck> OnUpdate;
        public event Action<string> OnDone;
        public event Action<string, Guid> OnSelect;
        public event Action<string, Guid> OnUnselect;
        public event Action<Speck[]> OnInitialize;

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
            _user = userName;
            _connection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromSeconds(10) })
                .Build();

            _connection.On<string, Speck>("Add", (user, speck) => OnAdd?.Invoke(user, speck));
            _connection.On<string, Guid>("Delete", (user, id) => OnDelete?.Invoke(user, id));
            _connection.On<string, Guid, Speck>("Update", (user, id, speck) => OnUpdate?.Invoke(user, id, speck));
            _connection.On<string>("Done", (user) => OnDone?.Invoke(user));
            _connection.On<string, Guid>("Select", (user, id) => OnSelect?.Invoke(user, id));
            _connection.On<string, Guid>("Unselect", (user, id) => OnUnselect?.Invoke(user, id));
            _connection.On<Speck[]>("Initialize", (specks) => OnInitialize?.Invoke(specks));
            _connection.Closed += Connection_Closed;
            _connection.Reconnecting += Connection_Reconnecting;

        }

        private Task Connection_Reconnecting(Exception arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        private Task Connection_Closed(Exception arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Update task
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="speck">speck</param>
        /// <returns></returns>
        public async Task Update(Guid id, Speck speck)
        {
            await _connection.InvokeAsync("Update", _user, id, speck);
        }

        /// <summary>
        /// Delete task
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>returns task</returns>
        public async Task Delete(Guid id)
        {
            await _connection.InvokeAsync("Delete", _user, id);
        }

        /// <summary>
        /// Add to database
        /// </summary>
        /// <param name="speck">speck</param>
        /// <returns>task</returns>
        public async Task Add(Speck speck)
        {
            await _connection.InvokeAsync("Add", _user, speck);
        }

        /// <summary>
        /// Done
        /// </summary>
        /// <returns></returns>
        public async Task Done()
        {
            await _connection.InvokeAsync("Done", _user);
        }

        /// <summary>
        /// Select event
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Select(Guid id)
        {
            await _connection.InvokeAsync("Select", _user, id);
        }

        /// <summary>
        /// Unselect event
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Unselect(Guid id)
        {
            await _connection.InvokeAsync("Unselect", _user, id);
        }

        /// <summary>
        /// Start the async connection
        /// </summary>
        /// <returns></returns>
        public Task StartAsync() => _connection.StartAsync();
    }
}