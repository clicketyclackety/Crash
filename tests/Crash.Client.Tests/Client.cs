using Crash.Common.Document;

namespace Crash.Client.Tests
{

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

	}

}
