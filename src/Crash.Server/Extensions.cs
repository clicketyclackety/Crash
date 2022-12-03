using Microsoft.EntityFrameworkCore;

namespace Crash.Server
{
    public static class Extensions
    {
        /// <summary>
        /// Creates new Database based on DbContext
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webHost"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IHost MigrateDatabase<T>(this IHost webHost) where T : DbContext
        {
            SQLitePCL.Batteries.Init();
            var serviceScopeFactory = (IServiceScopeFactory?)webHost
                .Services.GetService(typeof(IServiceScopeFactory));

            if (serviceScopeFactory == null)
                throw new InvalidOperationException("Cannot Get IServiceScopeFactory");

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
