using System;

namespace Framework.Essentials.TimeManagement
{
	public interface ITimeService
	{
		float DeltaTime { get; }
		float UnscaledDeltaTime { get; }
		DateTime UtcNow { get; }
		float Time { get; }
		float FixedDeltaTime { get; }
		float Slowdown { get; }
		bool IsPaused();
		PauseRequestHandler RequestPause();
		void ReleasePause(PauseRequestHandler handler);
		void SetSlowdown(float factor);
		event Action OnPause;
		event Action OnResume;
	}
}
