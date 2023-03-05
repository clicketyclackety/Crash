using Crash.Common.Document;
using Crash.Communications;

using Xunit;

namespace Crash.Server.Tests
{
	public sealed class FullTest
	{

		[Fact]
		public void StartServer()
		{
			string url = "http://localhost:8080";
			string[] args = new string[] { "--urls", url };

			// This needs to be called
			// Assert.DoesNotThrow(() => Program.Main(args));

			CrashDoc crashDoc = new CrashDoc();
			CrashServer crashServerManager = new CrashServer(crashDoc);
			Assert.DoesNotThrow(() => crashServerManager.Start(url));

			;

		}

	}

}
