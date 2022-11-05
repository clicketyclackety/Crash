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
        Task Initialize(Speck[] specks);
    }

    public class CrashHub : Hub<ICrashClient>
    {
        Model.CrashContext _context;
        public CrashHub(Model.CrashContext context)
        {
            _context = context;
        }


        public async Task Add(string user, Speck speck)
        {
            try
            {
                _context.Specks.Add(Model.Speck.From(speck));
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            await Clients.Others.Add(user, speck);
        }


        public async Task Update(string user, Guid id, Speck speck)
        {
            var oldSpeck = _context.Specks.FirstOrDefault(r => r.Id == id);

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


        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            //await Clients.Caller.Initialize(...);
        }
    }
}
