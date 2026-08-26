using System;
using UnityEngine;

namespace Code.Infrastructure.Satellite
{
	public class SatelliteService : ISatelliteService, IDebugSatelliteService
	{
		private bool _isForceOfflineEnabled;

		public event Action<bool> OnConnectionChanged;

		public bool IsForceOfflineEnabled() => _isForceOfflineEnabled;

		public void SetForceOfflineEnabled(bool enabled)
		{
			if (_isForceOfflineEnabled == enabled)
				return;

			_isForceOfflineEnabled = enabled;
			OnConnectionChanged?.Invoke(HasConnection());
		}

		public bool HasConnection()
		{
			if (_isForceOfflineEnabled)
				return false;

			return Application.internetReachability != NetworkReachability.NotReachable;
		}
	}
}
