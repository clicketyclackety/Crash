using Crash;

namespace ServerClient.Integration.Tests
{
    [TestClass]
    public class InvokeTests
    {

        const string TestName = "Jeremy";
        const string Port = "8080";
        const string LocalServer = $"https://localhost:{Port}";
        const string HostServer = $"https://0.0.0.0:{Port}";

        [TestMethod]
        public void Select()
        {
            CrashClient client = new CrashClient(TestName, new Uri(LocalServer));
            CrashServer server = new CrashServer();

            Crash.Server.CrashHub crashHub = new Crash.Server.CrashHub()
            

        }
    }
}