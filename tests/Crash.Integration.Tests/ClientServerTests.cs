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
			CrashDoc crashDoc = new CrashDoc();
			crashDoc.Users.CurrentUser = user;

			CrashServer crashServer = StartServer(crashDoc);
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
			Assert.DoesNotThrow(() => crashServer.Start(serverUrl));

			var messages = crashServer.Messages;

			Wait(() => crashServer.IsRunning);
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

			Assert.That(crashClient.IsConnected, "Client is not connected", Is.True);
			Assert.That(initRan, "Init did not run!", Is.True);

			return crashClient;
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
