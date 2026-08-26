using System;
using System.Collections.Generic;

namespace Code.Infrastructure.ErrorHandler
{
	public interface ILogGuardService
	{
		event Action OnFirstErrorHappened;
		event Action OnFirstExceptionHappened;
		bool HasError { get; }
		int ErrorCount { get; }
		int WarningCount { get; }
		bool HasException { get; }
		void Record(LogSeverity severity, string message, string stackTrace = null, string source = null);
		List<LogEntry> GetRecent(int maxCount = 200);
		string GetRecentAsString(int maxCount = 200);
		void ResetErrors(bool clearEntries = false);
		void AttachToUnityLogs();
	}
}