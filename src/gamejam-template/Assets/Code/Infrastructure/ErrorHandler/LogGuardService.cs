using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Code.Infrastructure.ErrorHandler
{
	public sealed class LogGuardService : IDisposable, ILogGuardService
	{
		private readonly object _lock = new();
		private readonly LogEntry[] _buffer;
		private int _nextIndex; // next write position
		private int _count; // number of valid entries in buffer
		private readonly int _capacity;
		private int _errorCount;
		private int _warningCount;
		private int _attached; // 0/1 guard for AttachToUnityLogs
		private volatile int _hasError; // 0/1
		private volatile int _hasException; // 0/1 

		public event Action OnFirstErrorHappened;
		public event Action OnFirstExceptionHappened;

		public LogGuardService(int capacity = 1024)
		{
			if (capacity <= 0) capacity = 256;
			_buffer = new LogEntry[capacity];
			_capacity = capacity;

			AttachToUnityLogs();
		}

		public bool HasError => _hasError == 1;
		public int ErrorCount => Volatile.Read(ref _errorCount);
		public int WarningCount => Volatile.Read(ref _warningCount);
		public bool HasException => _hasException == 1;

		public void Record(LogSeverity severity, string message, string stackTrace = null, string source = null)
		{
			LogEntry entry = new(DateTime.UtcNow, severity, message, stackTrace, source);

			// Update counters/flags first (lock-free) for quick paths
			if (severity >= LogSeverity.Error)
			{
				Interlocked.Increment(ref _errorCount);

				// First error (covers Exception as well)
				if (Interlocked.Exchange(ref _hasError, 1) == 0)
				{
					Action h1 = OnFirstErrorHappened;
					h1?.Invoke();
				}

				// First explicit Exception
				if (severity == LogSeverity.Exception &&
				    Interlocked.Exchange(ref _hasException, 1) == 0)
				{
					Action h2 = OnFirstExceptionHappened;
					h2?.Invoke();
				}
			}
			else if (severity == LogSeverity.Warning)
			{
				Interlocked.Increment(ref _warningCount);
			}

			// Store in ring buffer
			lock (_lock)
			{
				_buffer[_nextIndex] = entry;
				_nextIndex = (_nextIndex + 1) % _capacity;
				if (_count < _capacity) _count++;
			}
		}

		public List<LogEntry> GetRecent(int maxCount = 200)
		{
			if (maxCount <= 0) return new List<LogEntry>(0);
			List<LogEntry> list;
			lock (_lock)
			{
				int take = Math.Min(maxCount, _count);
				list = new List<LogEntry>(take);
				// iterate newest->oldest
				for (int i = 0; i < take; i++)
				{
					int idx = (_nextIndex - 1 - i);
					if (idx < 0) idx += _capacity;
					list.Add(_buffer[idx]);
				}
			}

			return list;
		}

		public string GetRecentAsString(int maxCount = 200)
		{
			List<LogEntry> entries = GetRecent(maxCount);
			return string.Join("\n", entries);
		}

		public void ResetErrors(bool clearEntries = false)
		{
			int hasError = _hasError;
			Volatile.Write(ref hasError, 0);
			int hasException = _hasException;
			Volatile.Write(ref hasException, 0);

			Interlocked.Exchange(ref _errorCount, 0);
			Interlocked.Exchange(ref _warningCount, 0);

			if (clearEntries)
			{
				lock (_lock)
				{
					_nextIndex = 0;
					_count = 0;
				}
			}
		}

		#if UNITY_2019_1_OR_NEWER
		public void AttachToUnityLogs()
		{
			if (Interlocked.Exchange(ref _attached, 1) == 1) return; // already attached
			Application.logMessageReceivedThreaded += HandleUnityLog;
		}

		private void HandleUnityLog(string condition, string stackTrace, LogType type)
		{
			Record(MapUnity(type), condition, stackTrace, source: "Unity");
		}
		#endif

		public void Dispose()
		{
			#if UNITY_2019_1_OR_NEWER
			if (Interlocked.Exchange(ref _attached, 0) == 1)
				Application.logMessageReceivedThreaded -= HandleUnityLog;
			#endif
		}

		private static LogSeverity MapUnity(
			#if UNITY_2019_1_OR_NEWER
			LogType type
			#else
        object type
			#endif
		)
		{
			#if UNITY_2019_1_OR_NEWER
			switch (type)
			{
				case LogType.Error: return LogSeverity.Error;
				case LogType.Assert: return LogSeverity.Assert;
				case LogType.Warning: return LogSeverity.Warning;
				case LogType.Log: return LogSeverity.Info;
				case LogType.Exception: return LogSeverity.Exception;
				default: return LogSeverity.Info;
			}
			#else
        return LogSeverity.Info;
			#endif
		}
	}
}