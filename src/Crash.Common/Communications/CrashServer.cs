using System.Diagnostics;
using System.IO;
using System.Threading;

using Crash.Common.Document;
using Crash.Common.Events;

namespace Crash.Communications
{
	/// <summary>
	/// Crash server class to handle the ServerProcess
	/// </summary>
	public sealed class CrashServer : IDisposable
	{
		public const int DefaultPort = 5000;
		public const string DefaultURL = "http://0.0.0.0";

		private CrashDoc _crashDoc;

		public Process? process { get; set; }

		public bool IsRunning => process is not null && !process.HasExited;
		public bool Connected { get; private set; }

		public const string ProcessName = "Crash.Server";

		/// <summary>
		/// Empty constructor
		/// </summary>
		public CrashServer(CrashDoc crashDoc)
		{
			_crashDoc = crashDoc;
		}

		public const string EXTRACTED_SERVER_FILENAME = $"{CrashServer.ProcessName}.exe";
		public static string BASE_DIRECTORY;
		public static string SERVER_DIRECTORY => Path.Combine(BASE_DIRECTORY, "Server");
		public static string SERVER_FILEPATH => Path.Combine(SERVER_DIRECTORY, EXTRACTED_SERVER_FILENAME);

		static CrashServer()
		{
			var app_data = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create);
			BASE_DIRECTORY = Path.Combine(app_data, "Crash");
		}

		~CrashServer()
		{
			Dispose(false);
		}

		public void CloseLocalServer()
		{
			this?.Stop();
			this?.Dispose();
		}

		/// <summary>
		/// Method to start the server
		/// </summary>
		/// <param name="url">The URL for the Server</param>
		/// <param name="isMac">Is the given OS Mac?</param>
		/// <param name="errorMessage">The error, if nay</param>
		/// <returns>True on success, false otherwise</returns>
		public void Start(string url) => Start(getStartInfo(getServerExecutablePath(), url));

		internal void Start(ProcessStartInfo startInfo, int timeout = 3000)
		{
			string errorMessage;
			if (checkForPreExistingServer())
			{
				errorMessage = "Server Process is already running!";
				throw new Exception(errorMessage);
			}

			try
			{
				createAndRegisterServerProcess(startInfo);
			}
			catch (FileNotFoundException)
			{
				errorMessage = "Could not find Server exe";
				throw new FileNotFoundException(errorMessage);
			}

			for (int i = 0; i <= timeout; i += 100)
			{
				if (Connected) break;
				Thread.Sleep(100);
			}
		}

		private bool checkForPreExistingServer()
		{
			var processes = Process.GetProcessesByName(ProcessName, Environment.MachineName);
			return processes.Length != 0;
		}

		// TODO : Unit Test this
		public static bool ForceCloselocalServers(int timeout = 0)
		{
			var processes = Process.GetProcessesByName(ProcessName, Environment.MachineName);
			if (null == processes || processes.Length == 0) return true;

			foreach (var serverProcess in processes)
			{
				serverProcess?.Kill();
			}

			// TODO : should this use a cancellation token?
			int step = 100;
			for (int i = 0; i < timeout; i++)
			{
				Thread.Sleep(step);
				i += step;
			}

			return processes.All(p => null == p || p.HasExited);
		}

		internal string getServerExecutablePath()
		{
			if (!File.Exists(SERVER_FILEPATH))
			{
				throw new DirectoryNotFoundException("Could not find server executable directory!");
			}

			return SERVER_FILEPATH;
		}

		// https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output
		internal ProcessStartInfo getStartInfo(string serverExecutable, string url)
		{
			var startInfo = new ProcessStartInfo()
			{
				FileName = serverExecutable,
				Arguments = $"--urls \"{url}\"",
				CreateNoWindow = true, // !Debugger.IsAttached,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
			};

			return startInfo;
		}

		// https://stackoverflow.com/questions/285760/how-to-spawn-a-process-and-capture-its-stdout-in-net
		internal void createAndRegisterServerProcess(ProcessStartInfo startInfo)
		{
			process = new Process
			{
				StartInfo = startInfo ?? throw new ArgumentNullException("Process Info is null"),
				EnableRaisingEvents = true
			};

			// Register fresh
			process.Disposed += Process_Exited;
			process.Exited += Process_Exited;
			process.OutputDataReceived += Process_OutputDataReceived;
			process.ErrorDataReceived += Process_ErrorDataReceived;

			if (!process.Start())
				throw new ApplicationException("Failed to start server!");

			if (startInfo.RedirectStandardOutput)
				process.BeginOutputReadLine();

			if (startInfo.RedirectStandardError)
				process.BeginErrorReadLine();
		}

		internal List<string> Messages = new List<string>();

		private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			var data = e.Data;
			if (string.IsNullOrEmpty(data)) return;
			Messages.Add(data);

			if (data.Contains("failed to bind") ||
				data.Contains("AddressInUseException") ||
				data.Contains("SocketException "))
			{
				var portInUseMessage = "Given Port is already in use! Try another!";
				Messages.Add(portInUseMessage);
			}
			else if (data.Contains("hostpolicy.dll"))
			{
				Messages.Add("Crash.Server.exe was referenced incorrectly");
			}

			process.ErrorDataReceived -= Process_ErrorDataReceived;
			OnFailure?.Invoke(this, null);
			Stop();
		}

		private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			var data = e.Data;
			if (string.IsNullOrEmpty(data)) return;
			Messages.Add(data);

			Console.WriteLine(data);
			var started = data.ToLower().Contains("now listening on: http");
			if (started)
			{
				Connected = true;
				OnConnected?.Invoke(this, new CrashEventArgs(_crashDoc));
			}
		}

		private void Process_Exited(object sender, EventArgs e)
		{
			// TODO : Capture Exit and either attempt restart or exit application
			Messages.Add("Exited!");
		}


		#region Stop/End

		/// <summary>
		/// Stop connection
		/// </summary>
		public void Stop()
		{
			try
			{
				if (process is not null)
				{
					// De-Register first to avoid duplicate calls
					process.Disposed -= Process_Exited;
					process.Exited -= Process_Exited;
					process.OutputDataReceived -= Process_OutputDataReceived;
					process.ErrorDataReceived -= Process_ErrorDataReceived;
				}

				process?.Kill();
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
