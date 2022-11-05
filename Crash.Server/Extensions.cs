using Microsoft.EntityFrameworkCore;

namespace Crash.Server
{
    public static class Extensions
    {
        public static IHost MigrateDatabase<T>(this IHost webHost) where T : DbContext
        {
            SQLitePCL.Batteries.Init();
            var serviceScopeFactory = (IServiceScopeFactory?)webHost
                .Services.GetService(typeof(IServiceScopeFactory));

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;

                var dbContext = services.GetRequiredService<T>();
                dbContext.Database.Migrate();
            }

            return webHost;
        }
    }
}
