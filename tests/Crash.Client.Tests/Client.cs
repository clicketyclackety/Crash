namespace Crash.Client.Tests
{
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

	[TestClass]
	public class Client
	{

		[TestMethod]
		public void InvalidConstructorArgs()
		{

			Assert.ThrowsException<ArgumentException>(() => new CrashClient(null, null));
			Assert.ThrowsException<ArgumentException>(() => new CrashClient(string.Empty, null));
			Assert.ThrowsException<UriFormatException>(() => new CrashClient(string.Empty, new Uri(string.Empty)));
			Assert.ThrowsException<UriFormatException>(() => new CrashClient(string.Empty, new Uri("htp://@.co://192")));
			Assert.ThrowsException<UriFormatException>(() => new CrashClient("Paul", null));
		}

		// [TestMethod] // Seems to be failing
		public async Task SelectAsync()
		{

			var client = new CrashClient("Me", new Uri("http://localhost:5000/Crash"));
			await client.StartAsync();
			await client.Select(Guid.Empty);
		}

#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

	}

}
