using System;

namespace Code.Gameplay.Suspicion.Queries
{
	public interface ISuspicionQuery
	{
		event Action<float> OnLevelChanged;

		float GetLevel();
		float GetMaximumLevel();
	}
}
