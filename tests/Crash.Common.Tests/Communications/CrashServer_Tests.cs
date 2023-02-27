using System.Diagnostics;

using Crash.Common.Document;
using Crash.Communications;

namespace Crash.Common.Tests.Communications
{

	public sealed class CrashServer_Tests : IDisposable
	{

		[SetUp]
		public void SetUp()
		{
			CrashServer.ForceCloselocalServers();
		}

		[TearDown]
		public void TearDown()
		{
			CrashServer.ForceCloselocalServers();
		}

		[Test]
		public void GetExePath()
		{
			CrashServer server = new CrashServer(new CrashDoc());
			string serverPath = server.getServerExecutablePath();

			Assert.That(File.Exists(serverPath), Is.True);
		}

		[Test]
		public void RegisterServerProcess()
		{
			CrashServer server = new CrashServer(new CrashDoc());
			string exe = server.getServerExecutablePath();
			string url = $"{CrashServer.DefaultURL}:{CrashServer.DefaultPort}";

			var startInfo = server.getStartInfo(exe, url);

			Assert.DoesNotThrow(() => server.createAndRegisterServerProcess(startInfo));

			Assert.That(server.process, Is.Not.Null);
			Assert.That(server.IsRunning, Is.True);
		}

		[Test]
		public void RegisterServerProcess_InvalidInputs()
		{
			CrashServer server = new CrashServer(new CrashDoc());
			Assert.Throws<ArgumentNullException>(() => server.createAndRegisterServerProcess(null));
		}

		[Test]
		public void StartServer()
		{
			string url = $"{CrashServer.DefaultURL}:{CrashServer.DefaultPort}";
			CrashServer server = new CrashServer(new CrashDoc());
			Assert.DoesNotThrow(() => server.Start(url));

			Assert.That(server.process, Is.Not.Null);
			Assert.That(server.IsRunning, Is.True);
		}

		[Test]
		public void VerifyFunctionalServer()
		{
			CrashServer server = new CrashServer(new CrashDoc());
			string exe = server.getServerExecutablePath();
			string url = $"{CrashServer.DefaultURL}:{CrashServer.DefaultPort}";

			var startInfo = new ProcessStartInfo()
			{
				FileName = exe,
				Arguments = $"--urls \"{url}\"",
				CreateNoWindow = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
			};

			Assert.DoesNotThrow(() => server.createAndRegisterServerProcess(startInfo));

			Assert.That(server.process, Is.Not.Null);
			Assert.That(server.IsRunning, Is.True);
		}

		public void Dispose()
		{
			CrashServer.ForceCloselocalServers();
		}
	}

}
