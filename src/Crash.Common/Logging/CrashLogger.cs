using System.Diagnostics;
using System.IO;

using Microsoft.Extensions.Logging;

namespace Crash.Common.Logging
{

	/// <summary>Enables logging for Crash</summary>
	public sealed class CrashLogger : ILogger, IDisposable
	{
		LogLevel _currentLevel;

		public static CrashLogger Logger { get; private set; }

		internal CrashLogger()
		{
			_currentLevel = Debugger.IsAttached ? LogLevel.Trace : LogLevel.Information;
		}

		static CrashLogger()
		{
			Logger = new CrashLogger();
		}

		public IDisposable BeginScope<TState>(TState state) => this;

		public void Dispose()
		{

		}

		public bool IsEnabled(LogLevel logLevel) => logLevel >= _currentLevel;

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			var _eventId = eventId.Name;
			var formattedMessage = formatter.Invoke(state, exception);
			var message = $"{logLevel} : {formattedMessage} : {_eventId}";

			LogFiles.writeLogMessage(message);
		}

		internal sealed class LogFiles
		{
			private static string _logDirectory;
			private static string _logFileName;
			private static string _logFilePath;

			private static CrashLogger _logger;

			internal LogFiles(CrashLogger logger)
			{
				_logger = logger;
			}

			static LogFiles()
			{
				var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				_logDirectory = Path.Combine(appData, "Crash", "Logs");
				_logFileName = $"{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}.log";
				_logFilePath = Path.Combine(_logDirectory, _logFileName);

				createLogFile();
			}

			private static void createLogFile()
			{
				if (!Directory.Exists(_logDirectory))
				{
					Directory.CreateDirectory(_logDirectory);
				}

				if (!File.Exists(_logFilePath))
				{
					File.Create(_logFilePath);
				}
			}

			internal static void writeLogMessage(string message)
			{
				try
				{
					File.AppendAllLines(_logFilePath, new string[] { message });
				}
				catch (Exception)
				{

				}
			}

		}

	}

}
