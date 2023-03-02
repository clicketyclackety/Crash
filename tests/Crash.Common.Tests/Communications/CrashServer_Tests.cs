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
			Assert.DoesNotThrow(() => server.Start(GetStartInfo(url)));

			var msgs = server.Messages;

			Assert.That(server.process, Is.Not.Null);
			Assert.That(server.IsRunning, Is.True);
		}

		private ProcessStartInfo GetStartInfo(string url)
		{
			string net60 = Path.GetDirectoryName(typeof(CrashServer_Tests).Assembly.Location);
			string debug = Path.GetDirectoryName(net60);
			string bin = Path.GetDirectoryName(debug);
			string project = Path.GetDirectoryName(bin);
			string tests = Path.GetDirectoryName(project);
			string source = Path.GetDirectoryName(tests);

			string crashServerPath = Path.Combine(source, "src", "Crash.Server", "bin", "debug");

			// C:\Users\csykes\Documents\cloned_gits\Crash\src\Crash.Server\bin\Debug
			// C:\Users\csykes\Documents\cloned_gits\Crash\tests\Crash.Integration.Tests\bin\Debug\net6.0

			string[] exes = Directory.GetFiles(crashServerPath, "Crash.Server.exe");
			string serverExecutable = exes.FirstOrDefault();
			string serverExePath = Path.GetDirectoryName(serverExecutable);
			string newDbName = $"{Guid.NewGuid()}.db";
			string dbPath = Path.Combine(net60, newDbName);

			var startInfo = new ProcessStartInfo()
			{
				FileName = serverExecutable,
				Arguments = $"--urls \"{url}\" --path \"{dbPath}\"",
				CreateNoWindow = true, // !Debugger.IsAttached,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
			};

			return startInfo;
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
