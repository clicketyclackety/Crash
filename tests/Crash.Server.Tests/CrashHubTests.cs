using System.Collections;

using Crash.Common.Changes;
using Crash.Common.View;
using Crash.Geometry;
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
			int currCount = _crashHub.Count;
			string testPayload = TestContext.CurrentContext.Random.NextGuid().ToString();
			Assert.That(change.Payload, Is.Not.EqualTo(testPayload));
			Change newChange = new Change(change.Id, change.Owner, testPayload);

			await _crashHub.Add(change.Owner, change);
			Assert.That(_crashHub.Count, Is.EqualTo(currCount + 1));
			currCount = _crashHub.Count;

			await _crashHub.Update(change.Owner, change.Id, newChange);
			Assert.That(_crashHub.Count, Is.EqualTo(currCount));

			Assert.That(_crashHub.TryGet(newChange.Id, out Change updatedChange), Is.True);
			Assert.That(updatedChange.Payload, Is.EqualTo(testPayload));
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

			await _crashHub.Unselect(change.Owner, change.Id);
			Assert.That(_crashHub.TryGet(change.Id, out Change updatedFound), Is.True);
			ChangeAction updatedAction = (ChangeAction)updatedFound.Action;

			Assert.That(updatedAction.HasFlag(ChangeAction.Unlock), Is.True);
		}

		[TestCaseSource(typeof(CameraChanges), nameof(CameraChanges.TestCases))]
		public async Task Test_CameraChange_Async(CameraChange cameraChange)
		{
			int previousCount = _crashHub.Count;
			Change change = new Change(cameraChange);
			await _crashHub.CameraChange(cameraChange.Owner, change);
			Assert.That(_crashHub.Count, Is.EqualTo(previousCount + 1));
		}

		/*
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
					Camera camera = new Camera(CPoint.Origin, new CPoint(1, 2, 3));
					yield return CameraChange.CreateNew(camera, "Jerry");
				}
			}
		}


	}

}
