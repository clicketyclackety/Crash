using System.Diagnostics;
using System.IO;

using Crash.Common.Document;
using Crash.Common.Events;

using Microsoft.AspNetCore.Connections;

namespace Crash.Communications
{
	/// <summary>
	/// Crash server class to handle the ServerProcess
	/// </summary>
	public sealed class CrashServer : IDisposable
	{
		public const int DefaultPort = 8080;
		public const string DefaultURL = "http://localhost";

		private CrashDoc _crashDoc;

		public Process? process { get; set; }

		public bool IsRunning => process is object && !process.HasExited;

		public const string ProcessName = "Crash.Server";

		/// <summary>
		/// Empty constructor
		/// </summary>
		public CrashServer(CrashDoc crashDoc)
		{
			_crashDoc = crashDoc;
		}

		~CrashServer()
		{
			Dispose(false);
		}

		public void CloseLocalServer(CrashDoc crashDoc)
		{
			CrashServer? server = crashDoc?.LocalServer;
			if (null == server) return;

			server?.Stop();
			server?.Dispose();
		}

		/// <summary>
		/// Method to start the server
		/// </summary>
		/// <param name="url">The URL for the Server</param>
		/// <param name="isMac">Is the given OS Mac?</param>
		/// <param name="errorMessage">The error, if nay</param>
		/// <returns>True on success, false otherwise</returns>
		public void Start(string url)
		{
			string errorMessage = "Server Started Successfully";

			if (checkForPreExistingServer())
			{
				errorMessage = "Server Process is already running!";
				throw new Exception(errorMessage);
			}

			try
			{
				var serverExecutable = getServerExecutablePath();
				createAndRegisterServerProcess(serverExecutable, url);
			}
			catch (FileNotFoundException)
			{
				errorMessage = "Could not find Server exe";
				throw new FileNotFoundException(errorMessage);
			}

		}

		private bool checkForPreExistingServer()
		{
			var processes = Process.GetProcessesByName(ProcessName, Environment.MachineName);
			return processes.Length != 0;
		}

		public static bool ForceCloselocalServers()
		{
			var processes = Process.GetProcessesByName(ProcessName, Environment.MachineName);
			if (null == processes || processes.Length == 0) return false;

			foreach (var serverProcess in processes)
			{
				serverProcess?.Kill();
			}

			return processes.All(p => null == p || p.HasExited);
		}

		private string getServerExecutablePath()
		{
			var currentPath = Path.GetDirectoryName(typeof(CrashServer).Assembly.Location);
			var serverExes = Directory.GetFiles(currentPath, $"{ProcessName}.exe", SearchOption.AllDirectories);

			if (null == serverExes || serverExes.Length == 0)
			{
				throw new FileNotFoundException("Could not find Server Eexcutable!");
			}

			var serverExecutable = serverExes.FirstOrDefault();
			return serverExecutable;
		}

		// https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output
		private ProcessStartInfo getstartInfo(string serverExecutable, string url)
		{
			var startInfo = new ProcessStartInfo()
			{
				FileName = serverExecutable,
				Arguments = $"--urls \"{url}\"",
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
			};
			return startInfo;
		}

		// https://stackoverflow.com/questions/285760/how-to-spawn-a-process-and-capture-its-stdout-in-net
		private void createAndRegisterServerProcess(string serverExecutable, string url)
		{
			process = new Process();
			process.StartInfo = getstartInfo(serverExecutable, url);
			process.EnableRaisingEvents = true;

			// Register fresh
			process.Disposed += Process_Exited;
			process.Exited += Process_Exited;
			process.OutputDataReceived += Process_OutputDataReceived;
			process.ErrorDataReceived += Process_ErrorDataReceived;

			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
		}


		private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			var data = e.Data;
			if (string.IsNullOrEmpty(data)) return;
			EventArgs args;

			if (data.Contains("failed to bind") ||
				data.Contains("AddressInUseException") ||
				data.Contains("SocketException "))
			{
				var portInUseMessage = "Given Port is already in use! Try another!";
				args = new ErrorEventArgs(new AddressInUseException(portInUseMessage));
			}
			else
			{
				args = new ErrorEventArgs(new Exception(data));
			}

			process.ErrorDataReceived -= Process_ErrorDataReceived;
			OnFailure?.Invoke(this, new CrashEventArgs(_crashDoc));
			Stop();
		}

		private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			var data = e.Data;
			if (string.IsNullOrEmpty(data)) return;

			Console.WriteLine(data);
			var started = data.ToLower().Contains("now listening on: http");
			if (started)
			{
				process.OutputDataReceived -= Process_OutputDataReceived;
				OnConnected?.Invoke(this, new CrashEventArgs(_crashDoc));
			}
		}

		private void Process_Exited(object sender, EventArgs e)
		{
			// TODO : Capture Exit and either attempt restart or exit application
			// Stop();
			;
		}


		#region Stop/End

		/// <summary>
		/// Stop connection
		/// </summary>
		public void Stop()
		{
			try
			{
				process?.Kill();
				if (process is object)
				{
					// De-Register first to avoid duplicate calls
					process.Disposed -= Process_Exited;
					process.Exited -= Process_Exited;
					process.OutputDataReceived -= Process_OutputDataReceived;
					process.ErrorDataReceived -= Process_ErrorDataReceived;
				}
			}
			catch
			{

			}
			finally
			{
				process = null;
			}
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose() => Dispose(true);

		/// <summary>
		/// Disposes of the object, stopping the server if it is running
		/// </summary>
		/// <param name="disposing">true if disposing, false if GC'd</param>
		public void Dispose(bool disposing)
		{
			// stop the server!
			Stop();
		}

		#endregion


		public event EventHandler<CrashEventArgs>? OnConnected;
		public event EventHandler<CrashEventArgs>? OnFailure;

	}
}
