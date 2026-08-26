using System;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Health
{
	public class ApplicationHealthService : IApplicationHealthService, IInitializable, IDisposable
	{
		public bool HasCriticalErrors { get; private set; }

		public void Initialize()
		{
			Application.logMessageReceived += HandleLogMessageReceived;
		}

		public void Dispose()
		{
			Application.logMessageReceived -= HandleLogMessageReceived;
		}

		private void HandleLogMessageReceived(string condition, string stackTrace, LogType type)
		{
			if (type == LogType.Exception)
			{
				HasCriticalErrors = true;
			}
		}
	}
}
