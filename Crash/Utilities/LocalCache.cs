using SpeckLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Utilities
{
    public sealed class LocalCache
    {
        private ConcurrentDictionary<Guid, Speck> _cache { get; set; }

        public static LocalCache Instance { get; set; }

        public LocalCache()
        {
        }

        
    }
}
