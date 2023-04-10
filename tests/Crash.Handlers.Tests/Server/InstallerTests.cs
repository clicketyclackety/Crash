using System.Threading.Tasks;

using Crash.Handlers.Server;

namespace Crash.Handlers.Tests.Server
{

	public sealed class InstallerTests
	{

		[Test]
		public async Task DownloadTests()
		{
			if (ServerInstaller.ServerExecutableExists)
			{
				ServerInstaller.RemoveOldServer();
				Assert.That(ServerInstaller.ServerExecutableExists, Is.False);
			}

			Assert.That(await ServerInstaller.EnsureServerExecutableExists(), Is.True);
			Assert.That(ServerInstaller.ServerExecutableExists, Is.True);
		}

	}

}
