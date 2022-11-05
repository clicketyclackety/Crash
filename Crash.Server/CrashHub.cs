using Crash.Server.Model;
using Microsoft.AspNetCore.SignalR;
using SpeckLib;

namespace Crash.Server
{

    public interface ICrashClient
    {
        Task Update(string user, Guid id, Speck speck);
        Task Add(string user, Speck speck);
        Task Delete(string user, Guid id);
        Task Done(string user);
        Task Select(string user, Guid id);
        Task Unselect(string user, Guid id);
    }

    public class CrashHub : Hub<ICrashClient>
    {
        CrashContext _context;
        public CrashHub(CrashContext context)
        {
            _context = context;
        }


        public async Task Add(string user, Speck speck)
        {
            try
            {
                _context.Specks.Add(speck);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Add: {ex}");
            }
            await Clients.Others.Add(user, speck);
        }

        public async Task Update(string user, Guid id, Speck speck)
        {
        }

        public async Task Delete(string user, Guid id)
        {

        }

        public async Task Done(string user)
        {

        }

        public async Task Select(string user, Guid id)
        {

        }

        public async Task Unselect(string user, Guid id)
        {

        }



        public override Task OnConnectedAsync()
        {
            // send 3dm and current state??

            return base.OnConnectedAsync();
        }
    }
}
