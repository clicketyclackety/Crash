using System.Collections;

using Crash.Common.Changes;
using Crash.Server.Model;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Crash.Server.Tests2
{

	public sealed class CrashHubTests
	{
		CrashHub _crashHub;
		CrashContext _crashContext;

		[SetUp]
		public void Init()
		{
			SetUpContext();
		}

		[TearDown]
		public void Cleanup()
		{
			_crashHub = null;
			_crashContext = null;
		}

		private void SetUpContext()
		{
			// Create a mock DbContextOptions object
			var mockOptions = new DbContextOptionsBuilder<CrashContext>()
				.UseInMemoryDatabase(databaseName: "test")
				.Options;

			_crashContext = new CrashContext(mockOptions);

			var hubContext = new Mock<ICrashClient>();
			var clientProxy = new Mock<IClientProxy>();
			var mockClientContext = new Mock<HubCallerContext>();

			_crashHub = new CrashHub(_crashContext);

			var mockClients = new Mock<IHubCallerClients<ICrashClient>>();
			var mockClientProxy_All = new Mock<ICrashClient>();
			var mockClientProxy_Others = new Mock<ICrashClient>();

			mockClients.Setup(clients => clients.All).Returns(mockClientProxy_All.Object);
			mockClients.Setup(clients => clients.Others).Returns(mockClientProxy_Others.Object);
			mockClientContext.Setup(c => c.ConnectionId).Returns(Guid.NewGuid().ToString());
			_crashHub.Clients = mockClients.Object;
			_crashHub.Context = mockClientContext.Object;
		}

		[Test]
		public void Test_Init()
		{

		}

		[TestCaseSource(typeof(Changes), nameof(Changes.TestCases))]
		public async Task Test_Add_Async(Change change)
		{
			int currCount = _crashHub.Count;
			await _crashHub.Add(change.Owner, change);
			Assert.That(_crashHub.Count, Is.EqualTo(currCount + 1));
		}

		[TestCaseSource(typeof(Changes), nameof(Changes.TestCases))]
		public async Task Test_Update_Async(Change change)
		{
			// Assert.That(_crashHub.Count, Is.EqualTo(0));
			await _crashHub.Update(change.Owner, change.Id, change);
			// Assert.That(_crashHub.Count, Is.EqualTo(1));
		}

		[TestCaseSource(typeof(Changes), nameof(Changes.TestCases))]
		public async Task Test_Delete_Async(Change change)
		{
			int currCount = _crashHub.Count;

			await _crashHub.Add(change.Owner, change);

			Assert.That(_crashHub.Count, Is.EqualTo(currCount + 1));

			await _crashHub.Delete(change.Owner, change.Id);

			Assert.That(_crashHub.Count, Is.EqualTo(currCount));
		}

		[TestCaseSource(typeof(Changes), nameof(Changes.TestCases))]
		public async Task Test_Done_Async(Change change)
		{
			// Assert.That(_crashHub.Count, Is.EqualTo(0));
			await _crashHub.Done(change.Owner);

			// Test recipients
			// _crashHub.Clients.All
		}

		[TestCaseSource(typeof(Changes), nameof(Changes.TestCases))]
		public async Task Test_Select_Async(Change change)
		{
			await _crashHub.Add(change.Owner, change);

			Assert.That(_crashHub.TryGet(change.Id, out Change found), Is.True);
			ChangeAction action = (ChangeAction)found.Action;
			Assert.That(action.HasFlag(ChangeAction.Lock), Is.False);

			await _crashHub.Select(change.Owner, change.Id);
			Assert.That(_crashHub.TryGet(change.Id, out Change updatedFound), Is.True);
			ChangeAction updatedAction = (ChangeAction)updatedFound.Action;

			Assert.That(updatedAction.HasFlag(ChangeAction.Lock), Is.True);
		}

		[TestCaseSource(typeof(Changes), nameof(Changes.TestCases))]
		public async Task Test_Unselect_Async(Change change)
		{
			await _crashHub.Add(change.Owner, change);

			Assert.That(_crashHub.TryGet(change.Id, out Change found), Is.True);
			ChangeAction action = (ChangeAction)found.Action;
			Assert.That(action.HasFlag(ChangeAction.Unlock), Is.False);

			await _crashHub.Select(change.Owner, change.Id);
			Assert.That(_crashHub.TryGet(change.Id, out Change updatedFound), Is.True);
			ChangeAction updatedAction = (ChangeAction)updatedFound.Action;

			Assert.That(updatedAction.HasFlag(ChangeAction.Unlock), Is.True);
		}

		[TestCaseSource(typeof(CameraChanges), nameof(CameraChanges.TestCases))]
		public async Task Test_CameraChange_Async(CameraChange cameraChange)
		{
			// Assert.That(_crashHub.Count, Is.EqualTo(0));
			// await _crashHub.CameraChange(cameraChange.Owner, cameraChange);
			// Assert.That(_crashHub.Count, Is.EqualTo(1));
		}


		/*
		[Test]
		public void TestFunction()
		{
			// Arrange
			var hubContext = new Mock<IHubCallerClients>();
			var clientProxy = new Mock<IClientProxy>();
			var connectionId = Guid.NewGuid().ToString();
			var httpContext = new DefaultHttpContext();
			httpContext.Connection.Id = connectionId;
			hubContext.Setup(x => x.Client(It.IsAny<string>())).Returns(clientProxy.Object);
			var myHub = new MyHub(hubContext.Object);

			// Act
			myHub.MyMethod("hello");

			// Assert
			;

			clientProxy.Verify(x => x.SendCoreAsync("MyMethod", new object[] { "hello" }, default(CancellationToken)), Times.Once);
		}
		*/

		/*
		[SetUp]
		public void Setup()
		{
			var argHandler = new ArgumentHandler();
			argHandler.EnsureDefaults();
			var optionsBuilder = new DbContextOptionsBuilder<CrashContext>();
			optionsBuilder.UseSqlite<CrashContext>();

			CrashContext context = new CrashContext(optionsBuilder.Options);
			CrashHub crashHub = new CrashHub(context);
			_crashHub = crashHub;
		}

		public async Task Setup()
		{
			string[] args = new string[] { };
			var builder = WebApplication.CreateBuilder(args);
			var argHandler = new ArgumentHandler();
			argHandler.EnsureDefaults();
			argHandler.ParseArgs(args);

			builder.Services.AddSignalR();

			builder.Services.AddDbContext<CrashContext>(options =>
						   options.UseSqlite($"Data Source={argHandler.DatabaseFileName}"));

			builder.WebHost.UseUrls(argHandler.URL);

			var app = builder.Build();

			// TODO : Make a nice little webpage
			app.MapGet("/", () => "Welcome to Crash!");
			app.MapHub<CrashHub>("/Crash");

			app.MigrateDatabase<CrashContext>();

			_webApp = app;
			// await app.RunAsync();
			// Tell Client we're ready!
		}

		[TestCaseSource(typeof(Changes), nameof(Changes.TestCases))]
		public async Task Test_Add(Change change)
		{
			int startCount = _crashHub.Count;

			// app.Urls // test getting these!

			await _crashHub.Add(change.Owner, change);

			Assert.That(startCount, Is.LessThan(_crashHub.Count));
		}

		[TestCaseSource(typeof(Changes), nameof(Changes.TestCases))]
		public async Task Test_Add2(Change change)
		{
			var app = _webApp;
			;

		}

		[Test]
		public async Task Get_URLs()
		{
			var urls = _webApp.Urls.ToArray();
			;
			Assert.That(urls.Length, Is.GreaterThan(0));
		}
		*/

		public sealed class Changes
		{
			public static IEnumerable TestCases
			{
				get
				{
					yield return new Change(Guid.NewGuid(), "James", "payload");
					yield return new Change(Guid.NewGuid(), "Jenny", "payload");
					yield return new Change(Guid.NewGuid(), "Jeff", "payload");
				}
			}
		}


		public sealed class CameraChanges
		{
			public static IEnumerable TestCases
			{
				get
				{
					yield return CameraChange.CreateNew(new Common.View.Camera(), "James");
				}
			}
		}


	}

}
