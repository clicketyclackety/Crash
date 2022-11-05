using Crash.Server.Model;
using Microsoft.AspNetCore.SignalR;
using SpeckLib;

namespace Crash.Server
{

    public interface ICrashClient
    {
        Task ReceiveMessage(string user, Speck speck);
    }

    public class CrashHub : Hub<ICrashClient>
    {
        CrashContext _context;
        public CrashHub(CrashContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string user, Speck speck)
            => await Clients.All.ReceiveMessage(user, speck);

        public async Task Poke()
        {
            var speck = new Speck();
            _context.Specks.Add(speck);
        }

        public override Task OnConnectedAsync()
        {
            // send 3dm and current state??

            return base.OnConnectedAsync();
        }
    }
}
