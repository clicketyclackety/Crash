using System.Diagnostics;
using System.Net;

using Crash.Client;
using Crash.Common.Document;
using Crash.Communications;

namespace Crash.Integration.Tests
{

	public sealed class ClientServerTests
	{
		string clientUrl => $"{CrashClient.DefaultURL}:{CrashServer.DefaultPort}";
		string clientEndpoint => $"{clientUrl}/Crash";
		string serverUrl => $"{CrashServer.DefaultURL}:{CrashServer.DefaultPort}";
		Uri clientUri => new Uri(clientEndpoint);

		const int timeout = 3000;
		const int pollTime = 100;

		static User user => new User("Marcio");

		CrashDoc _crashDoc;

		[SetUp]
		public void Setup()
		{
			if (_crashDoc?.LocalServer is object)
			{
				_crashDoc.LocalServer.CloseLocalServer();
				Thread.Sleep(1000);
			}

			Assert.That(CrashServer.ForceCloselocalServers(1000),
						"Not all server instances are closed",
						Is.True);
		}

		[TearDown]
		public void TearDown()
		{
			if (_crashDoc?.LocalServer is object)
			{
				_crashDoc.LocalServer.CloseLocalServer();
				Thread.Sleep(1000);
			}

			Assert.That(CrashServer.ForceCloselocalServers(1000),
						"Not all server instances are closed",
						Is.True);
		}

		[Test]
		public async Task ServerProcess()
		{
			bool onConnected = false;
			bool onFailure = false;

			_crashDoc = new CrashDoc();
			_crashDoc.Users.CurrentUser = user;
			_crashDoc.LocalServer = new CrashServer(_crashDoc);

			_crashDoc.LocalServer.OnConnected += (sender, args) => onConnected = true;
			_crashDoc.LocalServer.OnFailure += (sender, args) => onFailure = true;

			CrashServer crashServer = StartServer(_crashDoc);

			Assert.That(crashServer.Connected, Is.True);

			Assert.That(onConnected, Is.True, "Server Connection failed");
			Assert.That(onFailure, Is.False, "Server failed!");
			onConnected = false;
			onFailure = false;

			await EnsureSiteIsUp();

			crashServer.Stop();

			Assert.That(crashServer.process, Is.Null, "Process is not null");
			Assert.That(crashServer.IsRunning, Is.False, "Server process is still running");
			Assert.That(onConnected, Is.False, "Server Connection failed");
			Assert.That(onFailure, Is.False, "Server failed!");
		}

		[Test]
		public async Task ServerAndClient()
		{
			_crashDoc = new CrashDoc();
			_crashDoc.Users.CurrentUser = user;

			CrashServer crashServer = StartServer(_crashDoc);

			await EnsureSiteIsUp();

			CrashClient crashClient = await StartClientAsync(_crashDoc);
		}

		private async Task EnsureSiteIsUp()
		{

			// Perform a URL ping
			var httpClient = new HttpClient();
			var response = await httpClient.GetAsync(clientUrl);
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
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
			bool initRan = false;
			Action<IEnumerable<Change>> func = (changes) => initRan = true;

			CrashClient crashClient = new CrashClient(crashDoc, user.Name, clientUri);
			crashDoc.LocalClient = crashClient;

			// TODO : FIX INIT CHECK
			await crashClient.StartLocalClientAsync();

			// Wait(() => crashClient.IsConnected);
			Assert.That(crashClient.IsConnected, Is.True, "Client is not connected");

			Wait(() => initRan);
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
			string newDbName = "database.db";
			string dbPath = Path.Combine(net60, newDbName);

			var startInfo = new ProcessStartInfo()
			{
				FileName = serverExecutable,
				Arguments = $"--urls \"{url}\" --path \"{dbPath}\" --reset true",
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
				Thread.Sleep(pollTime);
				if (condition.Invoke()) break;
			}
		}

	}

}
