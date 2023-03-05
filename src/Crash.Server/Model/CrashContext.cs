using Microsoft.EntityFrameworkCore;

namespace Crash.Server.Model
{

	public interface ICrashContextFactory
	{
		CrashContext Create();
	}

	public interface ICrashContext
	{
		public DbSet<Change> Changes { get; }
	}

	/// <summary>
	/// Implementation of DbContext to be used as SqLite DB Session
	/// </summary>
	public sealed class CrashContext : DbContext, ICrashContext
	{
		public CrashContext(DbContextOptions<CrashContext> options)
			: base(options)
		{
		}

		public DbSet<Change> Changes { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			;
			//optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True");
			//optionsBuilder.UseSqlite();
		}


		public class CrashContextFactory : ICrashContextFactory
		{
			private readonly DbContextOptions<CrashContext> _options;

			public CrashContextFactory(DbContextOptions<CrashContext> options)
			{
				_options = options;
			}

			public CrashContext Create()
			{
				return new CrashContext(_options);
			}
		}

	}

}

