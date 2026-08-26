using System;

namespace Code.Infrastructure.ErrorHandler
{
	public readonly struct LogEntry
	{
		public readonly DateTime UtcTime;
		public readonly LogSeverity Severity;
		public readonly string Message;
		public readonly string StackTrace;
		public readonly string Source;

		public LogEntry(DateTime utcTime, LogSeverity severity, string message, string stackTrace, string source)
		{
			UtcTime = utcTime;
			Severity = severity;
			Message = message ?? string.Empty;
			StackTrace = stackTrace ?? string.Empty;
			Source = source ?? string.Empty;
		}

		public override string ToString() => $"[{UtcTime:O}] {Severity} {Source}: {Message}\n{StackTrace}";
	}
}