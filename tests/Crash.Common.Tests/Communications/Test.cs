using Crash.Client;
using Crash.Common.Document;

using Microsoft.AspNetCore.SignalR.Client;

using Moq;

namespace Crash.Common.Tests.Communications
{
	[TestFixture]
	public class CrashClientTests
	{
		private Mock<HubConnection> _mockConnection;
		private Mock<CrashClient> _mockClient;
		private CrashDoc _mockCrashDoc;
		private string _mockUserName;
		private Uri _mockUrl;

		[SetUp]
		public void Setup()
		{
			_mockConnection = new Mock<HubConnection>();
			_mockCrashDoc = new CrashDoc();
			_mockUserName = "testUser";
			_mockUrl = new Uri("https://example.com");

			// Set up the mock connection to return a specific state
			// _mockConnection.Setup(c => c.State).Returns(HubConnectionState.Connected);

			_mockClient = new Mock<CrashClient>();

			var validGuid = It.Is<Guid>(guid => guid != Guid.Empty);
			var validChange = It.Is<Change>(change => change != null);

			_mockClient.Setup(cClient => cClient.AddAsync(validChange)).Returns(Task.CompletedTask);
			_mockClient.Setup(cClient => cClient.DeleteAsync(validGuid)).Returns(Task.CompletedTask);

			_mockClient.Setup(cClient => cClient.SelectAsync(validGuid)).Returns(Task.CompletedTask);
			_mockClient.Setup(cClient => cClient.UnselectAsync(validGuid)).Returns(Task.CompletedTask);

			_mockClient.Setup(cClient => cClient.DoneAsync()).Returns(Task.CompletedTask);

			_mockClient.Setup(cClient => cClient.UpdateAsync(validGuid, validChange)).Returns(Task.CompletedTask);

			_mockClient.Setup(cClient => cClient.CameraChangeAsync(validChange)).Returns(Task.CompletedTask);

			_mockClient.Setup(cClient => cClient.StartAsync()).Returns(Task.CompletedTask);
			_mockClient.Setup(cClient => cClient.StopAsync()).Returns(Task.CompletedTask);

			_mockClient.SetupAdd(cClient => cClient.OnAdd += OnAddAction);
		}

		private void OnAddAction(string userName, Change change)
		{
			;

		}

		[Test]
		public async Task TestOnAddAction()
		{
			await _mockClient.Object.AddAsync(null);
			;
		}

		[Test]
		public void Constructor_NullCrashDoc_ThrowsArgumentNullException()
		{
			// Arrange
			CrashDoc invalidCrashDoc = null;

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => new CrashClient(invalidCrashDoc, _mockUserName, _mockUrl));
		}

		[Test]
		public void Constructor_NullUserName_ThrowsArgumentException()
		{
			// Arrange
			string userName = null;

			// Act & Assert
			Assert.Throws<ArgumentException>(() => new CrashClient(_mockCrashDoc, userName, _mockUrl));
		}

		[Test]
		public void Constructor_NullUrl_ThrowsUriFormatException()
		{
			// Arrange
			Uri url = null;

			// Act & Assert
			Assert.Throws<UriFormatException>(() => new CrashClient(_mockCrashDoc, _mockUserName, url));
		}

		[Test]
		public void StartOrContinueLocalClientAsync_NullCrashDoc_ReturnsTaskCompleted()
		{
			// Arrange
			CrashDoc crashDoc = null;

			// Act
			var result = CrashClient.StartOrContinueLocalClientAsync(crashDoc, _mockUrl, null);

			// Assert
			Assert.IsTrue(result.IsCompletedSuccessfully);
		}

		[Test]
		public async Task UpdateAsync_InvokesConnectionInvokeAsync()
		{
			// Arrange
			var crashClient = new CrashClient(_mockCrashDoc, _mockUserName, _mockUrl);
			var id = Guid.NewGuid();
			var change = new Change();

			// Act
			await crashClient.UpdateAsync(id, change);

			// Assert
			// _mockConnection.Verify(c => c.InvokeAsync("Update", _mockUserName, id, change), Times.Once);
		}

		// Add more unit tests as needed
	}

}
