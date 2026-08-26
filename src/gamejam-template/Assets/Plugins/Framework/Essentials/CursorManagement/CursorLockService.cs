using System;
using System.Collections.Generic;
using Framework.Essentials.ScriptableObjects;
using UnityEngine;

namespace Framework.Essentials.CursorManagement
{
	public class CursorLockService : ICursorLockService, IDisposable
	{
		private readonly List<CursorLockStateHandler> _requests = new();

		public LockPreferenceType LockPreference { get; private set; } = LockPreferenceType.RequestMeansLock;

		public bool IsCursorLocked => Cursor.lockState == CursorLockMode.Locked;
		public bool IsCursorUnlocked => Cursor.lockState == CursorLockMode.None;

		public void Dispose()
		{
			_requests.Clear();
			Cursor.lockState = CursorLockMode.None;
		}

		public void SetLockPreference(LockPreferenceType preference)
		{
			LockPreference = preference;
			RefreshLockState();
		}

		public CursorLockStateHandler Request()
		{
			CursorLockStateHandler stateHandler = new(IDUtility.GenerateID());
			_requests.Add(stateHandler);

			RefreshLockState();

			return stateHandler;
		}

		public void Release(CursorLockStateHandler handler)
		{
			if (_requests.Contains(handler) == false)
				return;

			_requests.Remove(handler);
			RefreshLockState();
		}

		private void RefreshLockState()
		{
			if (_requests.Count == 0)
			{
				Cursor.lockState = LockPreference == LockPreferenceType.RequestMeansLock
					? CursorLockMode.None
					: CursorLockMode.Locked;

				return;
			}

			Cursor.lockState = LockPreference == LockPreferenceType.RequestMeansLock
				? CursorLockMode.Locked
				: CursorLockMode.None;
		}
	}
}
