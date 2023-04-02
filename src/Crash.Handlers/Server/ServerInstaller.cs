using System.IO;
using System.Net;

using Crash.Communications;

using Rhino;

namespace Crash.Handlers.Server
{

	public static class ServerInstaller
	{
		// Put these into an application setting somewhere? // Move from Crash.rhp
		static string CRASH_SERVER_FILENAME = $"{CrashServer.ProcessName}.exe";
		const string CRASH_NAME = nameof(Crash);
		const string SERVER_DIR = "Server";
		static readonly string CRASH_SERVER_FILEPATH;

		public static bool ServerExecutableExists => File.Exists(CRASH_SERVER_FILEPATH);

		static ServerInstaller()
		{
			var app_data = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create);
			CRASH_SERVER_FILEPATH = Path.Combine(app_data, nameof(Crash), "Server", CRASH_SERVER_FILENAME);
		}

		public static async Task<bool> DownloadAsync()
		{
			var crashServerExeUrl = "url";

			if (ServerExecutableExists)
			{
				// Remove old file
			}

			Directory.CreateDirectory(Path.GetDirectoryName(CRASH_SERVER_FILEPATH));

			using (var client = new WebClient())
			{
				client.DownloadProgressChanged += Client_DownloadProgressChanged;
				var urcrashServerExeUri = new Uri(crashServerExeUrl);
				await client.DownloadFileTaskAsync(urcrashServerExeUri, CRASH_SERVER_FILEPATH);
			}

			return ServerExecutableExists;
		}

		private static void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			if (e.ProgressPercentage % 25 == 0)
			{
				RhinoApp.WriteLine($"{CRASH_SERVER_FILENAME} - Downloaded {e.ProgressPercentage} of file. {e.TotalBytesToReceive}/{e.BytesReceived} bytes recieved.");
			}
		}

		static bool Install()
		{
			return false;
		}


	}

}
