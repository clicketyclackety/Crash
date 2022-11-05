using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using SpeckLib;

namespace Crash.Server.Model
{
    public class CrashContext : DbContext
    {
        public CrashContext(DbContextOptions<CrashContext> options)
            : base(options)
        {
        }

        public DbSet<Speck> Specks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True");
            //optionsBuilder.UseSqlite();
        }
    }

}
