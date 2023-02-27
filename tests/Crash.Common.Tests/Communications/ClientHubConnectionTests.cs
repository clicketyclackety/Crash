using Crash.Client;

using Microsoft.AspNetCore.SignalR.Client;

using Moq;

namespace Crash.Common.Tests.Communications
{

	public sealed class ClientHubConnectionTests
	{

		[Test]
		public void TestHubConnection()
		{
			// Arrange
			Uri url = new Uri("https://example.com");
			HubConnection connection = CrashClient.getHubConnection(url);

			// Assert
			Assert.IsNotNull(connection);
		}

		[Test]
		public async Task Test()
		{
			// Arrange
			var mockConnection = new Mock<HubConnection>();
			mockConnection.Setup(m => m.StartAsync(default)).Returns(Task.CompletedTask);

			// Act
			string result = await mockConnection.Object.InvokeAsync<string>("SomeMethod");

			// Assert
			Assert.That(result, Is.EqualTo("Result"));
		}

	}

}
