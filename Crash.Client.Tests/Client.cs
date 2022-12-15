namespace Crash.Tests
{

    [TestClass]
    public class Client
    {

        [TestMethod]
        public void InvalidConstructorArgs()
        {
            Assert.ThrowsException<ArgumentException>( () => new CrashClient(null, null));
            Assert.ThrowsException<ArgumentException>(() => new CrashClient(string.Empty, null));
            Assert.ThrowsException<UriFormatException>(() => new CrashClient(string.Empty, new Uri(string.Empty)));
            Assert.ThrowsException<UriFormatException>(() => new CrashClient(string.Empty, new Uri("htp://@.co://192")));
            Assert.ThrowsException<UriFormatException>(() => new CrashClient("Paul", null));
        }

        [TestMethod]
        public async Task Select()
        {
            CrashClient client = new CrashClient("Me", new Uri("http://localhost:8080/Crash"));
            await client.Select(Guid.Empty);
        }

    }

}
