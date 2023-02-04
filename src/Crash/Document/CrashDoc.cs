using Crash.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Document
{
    
    public sealed class CrashDoc : IDisposable
    {

        public readonly UserTable Users;

        public static CrashDoc? ActiveDoc;

        public CrashDoc()
        {
            Users = new UserTable();
        }

        public void Dispose()
        {
            ActiveDoc = null;
        }

    }

}
