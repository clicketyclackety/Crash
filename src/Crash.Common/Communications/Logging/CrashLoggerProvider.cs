using Microsoft.Extensions.Logging;

namespace Crash.Common.Communications.Logging
{
	internal sealed class CrashLoggerProvider : ILoggerProvider
	{

		CrashLogger logger;
		LogFiles logFiles;

		public ILogger CreateLogger(string categoryName)
		{
			logger = new CrashLogger();
			logFiles = new LogFiles(logger);
			return logger;
		}

		public void Dispose()
		{
			logFiles.Dispose();
		}
	}

}
