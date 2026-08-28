using System;

namespace Code.Gameplay.Drilling.Queries
{
	public interface IDrillingQuery
	{
		event Action<float> OnDistanceChanged;
		event Action OnRunFinished;

		float GetDistance();
		float GetBestDistance();
	}
}
