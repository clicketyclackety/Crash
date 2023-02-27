using Crash.Common.Document;

namespace Crash.Client.Tests
{
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

	[TestClass]
	public class Client
	{

		[TestMethod]
		public void InvalidConstructorArgs()
		{
			CrashDoc crashDoc = new CrashDoc();
			Assert.ThrowsException<ArgumentException>(() => new CrashClient(crashDoc, null, null));
			Assert.ThrowsException<ArgumentException>(() => new CrashClient(crashDoc, string.Empty, null));
			Assert.ThrowsException<UriFormatException>(() => new CrashClient(crashDoc, string.Empty, new Uri(string.Empty)));
			Assert.ThrowsException<UriFormatException>(() => new CrashClient(crashDoc, string.Empty, new Uri("htp://@.co://192")));
			Assert.ThrowsException<UriFormatException>(() => new CrashClient(crashDoc, "Paul", null));
		}

		// [TestMethod] // Seems to be failing
		public async Task SelectAsync()
		{

			var crashDoc = new CrashDoc();
			var client = new CrashClient(crashDoc, "Me", new Uri("http://localhost:5000/Crash"));
			await client.StartAsync();
			await client.SelectAsync(Guid.Empty);
		}

#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

	}

}
