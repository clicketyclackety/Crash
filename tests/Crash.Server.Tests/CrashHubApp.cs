using Crash.Client;
using Crash.Server;
using Crash.Server.Model;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Crash.Changes;

namespace Crash.Server.Tests
{

    public sealed class CrashHubApp : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var root = new InMemoryDatabaseRoot();

            var args = new string[]
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
            var serverClient = application.CreateClient();

            var resp = await serverClient.GetAsync(url);
            resp.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task GetAdd_Test()
        {
            var application = new CrashHubApp();
            var server = application.CreateClient();
            await server.GetAsync(url);

            var uri = new Uri(url);
            var crashClient = new CrashClient(Environment.UserName, uri);

            ;
            await crashClient.StartAsync();


            var Change = new Change(Guid.NewGuid(), Environment.UserName, "Example Payload");
            crashClient.Add(Change);

            ;

        }

        [TestMethod]
        public async Task Integration_Test()
        {
            var server = new CrashServer();
            Assert.IsTrue(server.Start(url, out var errorMessage));
            Assert.IsTrue(errorMessage.Contains("Success"));

            var client = new CrashClient(Environment.UserName, new Uri(url));
            await client.StartAsync();

            var Change = new Change(Guid.NewGuid(), Environment.UserName, "Example Payload");
            ;
            await client.Add(Change);
        }

    }

}
