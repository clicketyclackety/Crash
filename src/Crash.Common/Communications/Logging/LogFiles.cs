using System.IO;

namespace Crash.Common.Communications.Logging
{

	internal sealed class LogFiles : IDisposable
	{
		private string _logDirectory;
		private string _logFileName;
		private string _logFilePath;

		private CrashLogger _logger;

		internal LogFiles(CrashLogger logger)
		{
			_logger = logger;
			var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			_logDirectory = Path.Combine(appData, "Crash", "Logs");
			_logFileName = $"{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}.log";
			_logFilePath = Path.Combine(_logDirectory, _logFileName);

			createLogFile();

			_logger.OnLoggingMessage += writeLogMessage;
		}

		private void createLogFile()
		{
			if (!Directory.Exists(_logDirectory))
			{
				Directory.CreateDirectory(_logDirectory);
			}

			if (!File.Exists(_logFileName))
			{
				File.Create(_logFileName);
			}
		}

		public void Dispose()
		{
			// Close the file?
		}

		private void writeLogMessage(object sender, CrashLogger.LoggingEvent e)
		{
			File.AppendAllLines(_logFilePath, new string[] { e.Message });
		}

	}

}
