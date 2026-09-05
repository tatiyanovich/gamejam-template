using System;

namespace Code.Gameplay.Bell.Queries
{
	public interface IBellQuery
	{
		event Action<float> OnTimeLeftChanged;
		event Action OnAnnounced;

		float GetTimeLeft();
		bool IsAnnounced();
	}
}
