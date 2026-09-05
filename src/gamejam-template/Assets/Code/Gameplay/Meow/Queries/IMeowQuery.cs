using System;

namespace Code.Gameplay.Meow.Queries
{
	public interface IMeowQuery
	{
		event Action<float> OnMicrophoneLevelChanged;

		float GetMicrophoneLevel();
		float GetThresholdLevel();
		float GetRearmLevel();
		float GetCooldownTimeLeft();
		float GetCooldownSeconds();
		bool IsArmed();
		bool IsOnCooldown();
		bool IsMicrophoneAvailable();
	}
}
