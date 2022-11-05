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
            try
            {
                _context.Specks.Remove(_context.Specks.FirstOrDefault(r => r.Id == id));
                _context.Specks.Add(Model.Speck.From(speck));
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            await Clients.Others.Update(user, id, speck);
        }

        public async Task Delete(string user, Guid id)
        {
            try
            {
                _context.Specks.Remove(_context.Specks.FirstOrDefault(r => r.Id == id));
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            await Clients.Others.Delete(user, id);
        }

        public async Task Done(string user)
        {
            try
            {
                List<Model.Speck> done = new List<Model.Speck>();
                foreach (var speck in _context.Specks)
                {
                    if (speck.LockedBy == user)
                    {
                        speck.Temporary = false;
                        speck.LockedBy = null;
                        done.Add(speck);
                    }
                }
                _context.Specks.UpdateRange(done);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            await Clients.Others.Done(user);

        }

        public async Task Select(string user, Guid id)
        {
            try
            {
                Model.Speck modSpec = _context.Specks.FirstOrDefault(r => r.Id == id);
                modSpec.Temporary = true;
                modSpec.LockedBy = user;
                _context.Specks.Update(modSpec);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            await Clients.Others.Select(user, id);
        }

        public async Task Unselect(string user, Guid id)
        {
            try
            {
                Model.Speck unSelect = _context.Specks.FirstOrDefault(r => r.Id == id);
                unSelect.Temporary = false;
                unSelect.LockedBy = null;
                _context.Specks.Update(unSelect);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            await Clients.Others.Unselect(user, id);
        }



        public override Task OnConnectedAsync()
        {
            // send 3dm and current state??

            return base.OnConnectedAsync();
        }
    }
}
