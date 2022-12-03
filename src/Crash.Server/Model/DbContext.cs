using Microsoft.EntityFrameworkCore;

namespace Crash.Server.Model
{
    /// <summary>
    /// Implementation of DbContext to be used as SqLite DB Session
    /// </summary>
    public class CrashContext : DbContext
    {
        public CrashContext(DbContextOptions<CrashContext> options)
            : base(options)
        {
        }

        public DbSet<SpeckLib.Speck> Specks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True");
            //optionsBuilder.UseSqlite();
        }
    }

}
