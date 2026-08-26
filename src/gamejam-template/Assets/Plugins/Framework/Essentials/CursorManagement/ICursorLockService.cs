namespace Framework.Essentials.CursorManagement
{
	public interface ICursorLockService
	{
		CursorLockStateHandler Request();
		LockPreferenceType LockPreference { get; }
		bool IsCursorLocked { get; }
		bool IsCursorUnlocked { get; }
		void Release(CursorLockStateHandler handler);
		void SetLockPreference(LockPreferenceType preference);
	}
}
