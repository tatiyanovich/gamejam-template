using System;
using Framework.Essentials.TimeManagement;

namespace Code.Editor
{
	public class PlaytestTimeService : ITimeService
	{
		public float DeltaTime { get; set; }
		public float UnscaledDeltaTime => DeltaTime;
		public DateTime UtcNow => DateTime.UnixEpoch;
		public float Time => 0f;
		public float FixedDeltaTime => DeltaTime;
		public float Slowdown => 1f;

		public event Action OnPause { add { } remove { } }
		public event Action OnResume { add { } remove { } }

		public bool IsPaused() => false;
		public PauseRequestHandler RequestPause() => throw new NotSupportedException();
		public void ReleasePause(PauseRequestHandler handler) => throw new NotSupportedException();
		public void SetSlowdown(float factor) => throw new NotSupportedException();
	}
}
