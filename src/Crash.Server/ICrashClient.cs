﻿using Crash.Changes;

namespace Crash.Server
{
    /// <summary>
    /// EndPoints Interface
    /// </summary>
    public interface ICrashClient
    {
        Task Update(string user, Guid id, Change Change);
        Task Add(string user, Change Change);
        Task Delete(string user, Guid id);
        Task Done(string user);
        Task Select(string user, Guid id);
        Task Unselect(string user, Guid id);
        Task Initialize(Change[] Changes);
        Task CameraChange(string user, Change Change);
    }
}
