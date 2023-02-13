using Crash.Server;
using Crash.Server.Model;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SpeckLib;

namespace Crash.Tests
{

    public sealed class CrashHubApp : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var root = new InMemoryDatabaseRoot();

            string[] args = new string[]
            {
                "--urls ", "http://0.0.0.0:54625",
            };
            builder.ConfigureDefaults(args);
            builder.ConfigureServices(services =>
            {
                services.AddScoped(sp =>
                {
                    return new DbContextOptionsBuilder<CrashContext>()
                    // .UseInMemoryDatabase<CrashContext>("Tests", root)
                    .UseApplicationServiceProvider(sp)
                    .Options;
                });
            });

            return base.CreateHost(builder);
        }

    }

    [TestClass]
    public class UnitTest1
    {
        const string url = "http://localhost:54625";

        [TestMethod]
        public async Task StartServer()
        {
            var application = new CrashHubApp();
            HttpClient serverClient = application.CreateClient();

            var resp = await serverClient.GetAsync(url);
            resp.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task GetAdd_Test()
        {
            var application = new CrashHubApp();
            HttpClient server = application.CreateClient();
            await server.GetAsync(url);

            Uri uri = new Uri(url);
            CrashClient crashClient = new CrashClient(Environment.UserName, uri);

            ;
            await crashClient.StartAsync();


            Speck speck = new Speck(Guid.NewGuid(), Environment.UserName, "Example Payload");
            crashClient.Add(speck);

            ;

        }

        [TestMethod]
        public async Task Integration_Test()
        {
            CrashServer server = new CrashServer();
            Assert.IsTrue(server.Start(url, out string errorMessage));
            Assert.IsTrue(errorMessage.Contains("Success"));

            CrashClient client = new CrashClient(Environment.UserName, new Uri(url));
            await client.StartAsync();

            Speck speck = new Speck(Guid.NewGuid(), Environment.UserName, "Example Payload");
            ;
            await client.Add(speck);
        }

    }

}
