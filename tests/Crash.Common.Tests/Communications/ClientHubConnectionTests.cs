using Crash.Client;

using Microsoft.AspNetCore.SignalR.Client;

namespace Crash.Common.Tests.Communications
{

	public sealed class ClientHubConnectionTests
	{

		[Test]
		public void TestHubConnection()
		{
			// Arrange
			Uri url = new Uri("https://example.com");
			HubConnection connection = CrashClient.GetHubConnection(url);

			// Assert
			Assert.IsNotNull(connection);
		}

	}

}
