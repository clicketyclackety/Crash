using SpeckLib;

namespace Crash.Server
{
    /// <summary>
    /// EndPoints Interface
    /// </summary>
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
}
