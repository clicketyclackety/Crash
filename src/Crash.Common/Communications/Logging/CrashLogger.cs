using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace Crash.Common.Communications.Logging
{

	/// <summary>Enables logging for Crash</summary>
	public sealed class CrashLogger : ILogger, IDisposable
	{
		static LogLevel _currentLevel;

		static CrashLogger()
		{
			_currentLevel = Debugger.IsAttached ? LogLevel.Trace : LogLevel.Information;
		}

		public IDisposable BeginScope<TState>(TState state) => this;

		public bool IsEnabled(LogLevel logLevel) => logLevel >= _currentLevel;

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			var _eventId = eventId.Name;
			var formattedMessage = formatter.Invoke(state, exception);
			var message = $"{logLevel} : {formattedMessage} : {_eventId}";

			OnLoggingMessage?.Invoke(this, new LoggingEvent(message, logLevel));
		}

		public void Dispose() { }

		public event EventHandler<LoggingEvent> OnLoggingMessage;

		public sealed class LoggingEvent
		{
			public readonly string Message;
			public readonly LogLevel Level;
			internal LoggingEvent(string message, LogLevel logLevel)
			{
				Message = message;
				Level = logLevel;
			}
		}

	}

}
