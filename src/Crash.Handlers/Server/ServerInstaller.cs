using System.IO;
using System.IO.Compression;
using System.Net;

using Crash.Communications;

using Microsoft.VisualStudio.Threading;

using Rhino;

namespace Crash.Handlers.Server
{

	/// <summary>Class for handling Server Install</summary>
	public static class ServerInstaller
	{
		private const string ARCHIVED_SERVER_FILENAME = "crash.server.zip";
		private const string VERSION = "latest";
		private const string ARCHIVED_SERVER_DOWNLOAD_URL = $"https://github.com/crashcloud/crash.server/releases/{VERSION}/download/{ARCHIVED_SERVER_FILENAME}";
		private static string DOWNLOADED_FILEPATH => Path.Combine(CrashServer.BASE_DIRECTORY, ARCHIVED_SERVER_FILENAME);

		/// <summary>Checks for an exiting Crash.Server.exe</summary>
		public static bool ServerExecutableExists => File.Exists(CrashServer.SERVER_FILEPATH) &&
			new FileInfo(CrashServer.SERVER_FILEPATH).Length > 10_000;
		private static bool ServerExecutableExistsAndIsInvalid => File.Exists(CrashServer.SERVER_FILEPATH) &&
			new FileInfo(CrashServer.SERVER_FILEPATH).Length < 10_000;

		/// <summary>Checks for a server exe, and it doesn't exist, downloads it</summary>
		public static async Task<bool> EnsureServerExecutableExists()
		{
			if (ServerExecutableExists)
			{
				return true;
			}

			if (!File.Exists(DOWNLOADED_FILEPATH))
			{
				await DownloadAsync();
			}
			if (File.Exists(DOWNLOADED_FILEPATH))
			{
				RemoveOldServer();
				ExtractDownloadAsync();
				RemoveDownloadedArchive();
			}

			return ServerExecutableExists;
		}

		internal static void RemoveOldServer()
		{
			Directory.Delete(CrashServer.SERVER_DIRECTORY, true);
			Directory.CreateDirectory(CrashServer.SERVER_DIRECTORY);
		}

		internal static async Task<bool> DownloadAsync()
		{
			TimeSpan cancelSpan = TimeSpan.FromSeconds(60);
			if (ServerExecutableExists) return true;
			if (ServerExecutableExistsAndIsInvalid)
			{
				RemoveDownloadedArchive();
			}

			try
			{
				// Use HttpClient
				using (var client = new WebClient())
				{
					client.DownloadProgressChanged += Client_DownloadProgressChanged;
					var urcrashServerExeUri = new Uri(ARCHIVED_SERVER_DOWNLOAD_URL);
					await client.DownloadFileTaskAsync(urcrashServerExeUri, DOWNLOADED_FILEPATH).WithTimeout(cancelSpan);
				}

				if (ServerExecutableExistsAndIsInvalid)
				{
					throw new Exception($"Server download is corrupted!");
				}
			}
			catch (TimeoutException)
			{
				return false;
			}

			return ServerExecutableExists;
		}

		internal static void ExtractDownloadAsync()
		{
			ZipFile.ExtractToDirectory(DOWNLOADED_FILEPATH, CrashServer.SERVER_DIRECTORY);
		}

		private static void RemoveDownloadedArchive()
		{
			File.Delete(DOWNLOADED_FILEPATH);
		}

		private static Dictionary<int, bool> progessSoFar = new Dictionary<int, bool>();
		private static void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			const int segment = 25;
			int percentage = e.ProgressPercentage;
			int chunk_segment = (percentage / segment) * segment;

			if (!progessSoFar.ContainsKey(chunk_segment))
			{
				progessSoFar.Add(chunk_segment, true);

				RhinoApp.WriteLine($"{ARCHIVED_SERVER_FILENAME} - Downloaded {e.ProgressPercentage}% of file. {e.TotalBytesToReceive}/{e.BytesReceived} bytes recieved.");
			}
		}

	}

}
