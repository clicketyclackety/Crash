using Microsoft.AspNetCore.SignalR;
using SpeckLib;

namespace Crash.Server
{

    /// <summary>
    /// Server Implementation of ICrashClient EndPoints
    /// </summary>
    public sealed class CrashHub : Hub<ICrashClient>
    {
        Model.CrashContext _context;

        /// <summary>
        /// Initialize with SqLite DB
        /// </summary>
        /// <param name="context"></param>
        public CrashHub(Model.CrashContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Add Speck to SqLite DB and notify other clients
        /// </summary>
        /// <param name="user"></param>
        /// <param name="speck"></param>
        /// <returns></returns>
        public async Task Add(string user, Speck speck)
        {
            try
            {
                _context.Specks.Add(speck);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }

            await Clients.Others.Add(user, new Speck(speck));
        }

        /// <summary>
        /// Update Item in SqLite DB and notify other clients
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <param name="speck"></param>
        /// <returns></returns>
        public async Task Update(string user, Guid id, Speck speck)
        {
            try
            {
                var removeSpeck = _context.Specks.FirstOrDefault(r => r.Id == id);
                if (removeSpeck != null)
                {
                    _context.Specks.Remove(removeSpeck);
                }
                _context.Specks.Add(new Speck(speck));
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            await Clients.Others.Update(user, id, speck);
        }

        /// <summary>
        /// Delete Item in SqLite DB and notify other clients
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Delete(string user, Guid id)
        {
            try
            {
                var speck = _context.Specks.FirstOrDefault(r => r.Id == id);
                if (speck == null)
                    return;
                _context.Specks.Remove(speck);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            await Clients.Others.Delete(user, id);
        }

        /// <summary>
        /// Unlock Item in SqLite DB and notify other clients
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task Done(string user)
        {
            try
            {
                List<Speck> done = new List<Speck>();
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

        /// <summary>
        /// Lock Item in SqLite DB and notify other clients
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Select(string user, Guid id)
        {
            try
            {
                var modSpec = _context.Specks.FirstOrDefault(r => r.Id == id);
                if (modSpec == null)
                    return;
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

        /// <summary>
        /// Unlock Item in SqLite DB and notify other clients
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Unselect(string user, Guid id)
        {
            try
            {
                var unSelect = _context.Specks.FirstOrDefault(r => r.Id == id);
                if (unSelect == null)
                    return;
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

        /// <summary>
        /// On Connected send user specks from DB
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var specks = _context.Specks.ToArray();
            await Clients.Caller.Initialize(specks);
        }
    }
}
