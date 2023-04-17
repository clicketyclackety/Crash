using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;

using Crash.Communications;

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
			new FileInfo(CrashServer.SERVER_FILEPATH).Length > 100_000;
		private static bool ServerExecutableExistsAndIsInvalid => File.Exists(CrashServer.SERVER_FILEPATH) &&
			new FileInfo(CrashServer.SERVER_FILEPATH).Length < 100_000;

		/// <summary>Checks for a server exe, and it doesn't exist, downloads it</summary>
		public static async Task<bool> EnsureServerExecutableExists()
		{
			if (ServerExecutableExists)
			{
				return true;
			}

			bool downloaded = false;
			if (!File.Exists(DOWNLOADED_FILEPATH))
			{
				RemoveOldServer();
				downloaded = await DownloadAsync();
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
			if (Directory.Exists(CrashServer.SERVER_DIRECTORY))
			{
				Directory.Delete(CrashServer.SERVER_DIRECTORY, true);
			}
			if (!Directory.Exists(CrashServer.SERVER_DIRECTORY))
			{
				Directory.CreateDirectory(CrashServer.SERVER_DIRECTORY);
			}
		}

		internal static async Task<bool> DownloadAsync()
		{
			TimeSpan cancelSpan = TimeSpan.FromSeconds(300);
			if (ServerExecutableExists) return true;
			if (ServerExecutableExistsAndIsInvalid)
			{
				RemoveDownloadedArchive();
			}

			try
			{
				Progress<float> progress = new Progress<float>();
				progress.ProgressChanged += (send, perc) => Client_DownloadProgressChanged(perc);

				// Seting up the http client used to download the data
				using (var client = new HttpClient())
				{
					client.Timeout = TimeSpan.FromMinutes(5);

					// Create a file stream to store the downloaded data.
					// This really can be any type of writeable stream.
					using (var file = new FileStream(DOWNLOADED_FILEPATH, FileMode.Create, FileAccess.Write, FileShare.None))
					{

						// Use the custom extension method below to download the data.
						// The passed progress-instance will receive the download status updates.
						await client.DownloadAsync(ARCHIVED_SERVER_DOWNLOAD_URL, file, progress);
					}
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
		private static void Client_DownloadProgressChanged(float progressPercentage)
		{
			int chunk_segment = (int)(progressPercentage * 100);
			if (chunk_segment % 10 != 0) return;

			if (!progessSoFar.ContainsKey(chunk_segment))
			{
				progessSoFar.Add(chunk_segment, true);

				RhinoApp.WriteLine($"{ARCHIVED_SERVER_FILENAME} - Downloaded {chunk_segment}% of file.");
			}
		}

	}

	// https://stackoverflow.com/questions/20661652/progress-bar-with-httpclient
	internal static class HttpClientExtensions
	{
		internal static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<float> progress = null, CancellationToken cancellationToken = default)
		{
			// Get the http headers first to examine the content length
			using (var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead))
			{
				var contentLength = response.Content.Headers.ContentLength;

				using (var download = await response.Content.ReadAsStreamAsync())
				{

					// Ignore progress reporting when no progress reporter was 
					// passed or when the content length is unknown
					if (progress == null || !contentLength.HasValue)
					{
						await download.CopyToAsync(destination);
						return;
					}

					// Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
					var relativeProgress = new Progress<long>(totalBytes => progress.Report((float)totalBytes / contentLength.Value));
					// Use extension method to report progress while downloading
					await download.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);
					progress.Report(1);
				}
			}
		}
	}

	internal static class StreamExtensions
	{
		public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long> progress = null, CancellationToken cancellationToken = default)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (!source.CanRead)
				throw new ArgumentException("Has to be readable", nameof(source));
			if (destination == null)
				throw new ArgumentNullException(nameof(destination));
			if (!destination.CanWrite)
				throw new ArgumentException("Has to be writable", nameof(destination));
			if (bufferSize < 0)
				throw new ArgumentOutOfRangeException(nameof(bufferSize));

			var buffer = new byte[bufferSize];
			long totalBytesRead = 0;
			int bytesRead;
			while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
			{
				await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
				totalBytesRead += bytesRead;
				progress?.Report(totalBytesRead);
			}
		}
	}

}
