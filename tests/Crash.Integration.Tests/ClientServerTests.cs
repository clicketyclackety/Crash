using System.Diagnostics;

using Crash.Client;
using Crash.Common.Document;
using Crash.Communications;

namespace Crash.Integration.Tests
{

	public sealed class ClientServerTests
	{
		const string port = "8080";
		const string clientUrl = $"http://127.0.0.1:{port}/Crash";
		const string serverUrl = $"http://0.0.0.0:{port}";
		static Uri uri = new Uri(clientUrl);

		const int timeout = 3000;
		const int pollTime = 100;

		static User user => new User("Marcio");


		[SetUp]
		public void Setup()
		{
			CrashServer.ForceCloselocalServers();
		}

		[Test]
		public async Task StartClient()
		{
			CrashDoc crashDoc = new CrashDoc();
			crashDoc.Users.CurrentUser = user;

			CrashClient crashClient = await StartClientAsync(crashDoc);
		}

		[Test]
		public void ServerProcess()
		{
			bool onConnected = false;
			bool onFailure = false;

			CrashDoc crashDoc = new CrashDoc();
			crashDoc.Users.CurrentUser = user;
			crashDoc.LocalServer = new CrashServer(crashDoc);

			crashDoc.LocalServer.OnConnected += (sender, args) => onConnected = true;
			crashDoc.LocalServer.OnFailure += (sender, args) => onFailure = true;

			CrashServer crashServer = StartServer(crashDoc);

			Assert.That(crashServer.Connected, Is.True);

			Assert.That(onConnected, Is.True, "Server Connection failed");
			Assert.That(onFailure, Is.False, "Server failed!");
			onConnected = false;
			onFailure = false;

			crashServer.Stop();

			Assert.That(crashServer.process, Is.Null, "Process is not null");
			Assert.That(crashServer.IsRunning, Is.False, "Server process is still running");
			Assert.That(onConnected, Is.False, "Server Connection failed");
			Assert.That(onFailure, Is.False, "Server failed!");
		}

		[Test]
		public async Task EndToEndAsync()
		{
			CrashDoc crashDoc = new CrashDoc();
			crashDoc.Users.CurrentUser = user;

			CrashServer crashServer = StartServer(crashDoc);

			CrashClient crashClient = await StartClientAsync(crashDoc);
		}

		private CrashServer StartServer(CrashDoc crashDoc)
		{
			crashDoc.Users.CurrentUser = user;

			CrashServer crashServer = crashDoc.LocalServer ?? new CrashServer(crashDoc);

			var processInfo = GetStartInfo(serverUrl);
			Assert.DoesNotThrow(() => crashServer.Start(processInfo));

			var messages = crashServer.Messages;
			Assert.That(crashServer.IsRunning, string.Join("\r\n", messages), Is.True);
			Assert.That(crashServer.process, Is.Not.Null);

			return crashServer;
		}

		private async Task<CrashClient> StartClientAsync(CrashDoc crashDoc)
		{
			// Arbitrarys
			Thread.Sleep(timeout);

			CrashClient crashClient = crashDoc.LocalClient ?? new CrashClient(crashDoc, user.Name, uri);

			bool initRan = false;
			Action<IEnumerable<Change>> func = (changes) => initRan = true;
			await CrashClient.StartOrContinueLocalClientAsync(crashDoc, uri, func);

			Wait(() => crashClient.IsConnected);

			Assert.That(crashClient.IsConnected, Is.True, "Client is not connected");
			Assert.That(initRan, Is.True, "Init did not run!");

			return crashClient;
		}

		private ProcessStartInfo GetStartInfo(string url)
		{
			string net60 = Path.GetDirectoryName(typeof(ClientServerTests).Assembly.Location);
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


		private void Wait(Func<bool> condition)
		{
			for (int i = 0; i < timeout; i += pollTime)
			{
				if (condition.Invoke()) break;
			}
		}

	}

}
