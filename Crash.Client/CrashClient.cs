using Microsoft.AspNetCore.SignalR.Client;
using SpeckLib;
using System;
using System.Threading.Tasks;

namespace Crash
{
    public class CrashClient
    {
        HubConnection _connection;
        string _user;

        public event Func<Exception, Task> Closed
        {
            add => _connection.Closed += value;
            remove => _connection.Closed -= value;
        }

        public Task StopAsync() => _connection.StopAsync();

        public CrashClient(string userName, Uri url)
        {
            _user = userName;
            _connection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromSeconds(10) })
                .Build();

            _connection.On<string, Speck>("Add", (user, speck) =>
            {
            });
            _connection.On<string, Guid>("Delete", (user, id) =>
            {
            });
            _connection.On<string, Guid, Speck>("Update", (user, id, speck) =>
            {
            });
            _connection.On<string, Guid[]>("Done", (user, speckles) =>
            {
            });
            _connection.On<string, Guid>("Select", (user, id) =>
            {
            });
            _connection.On<string, Guid>("Unselect", (user, id) =>
            {
            });
        }

        public async Task Update(Guid id, Speck speck)
        {
            await _connection.InvokeAsync("Update", _user, id, speck);
        }

        public async Task Delete(Guid id)
        {
            await _connection.InvokeAsync("Delete", _user, id);
        }

        public async Task Add(Speck speck)
        {
            await _connection.InvokeAsync("Add", _user, speck);
        }

        public async Task Done()
        {
            await _connection.InvokeAsync("Done", _user);
        }
        public async Task Select(Guid id)
        {
            await _connection.InvokeAsync("Select", _user, id);
        }
        public async Task Unselect(Guid id)
        {
            await _connection.InvokeAsync("Unselect", _user, id);
        }

        public Task StartAsync() => _connection.StartAsync();
    }
}