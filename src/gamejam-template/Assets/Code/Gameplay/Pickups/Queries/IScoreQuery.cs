using System;

namespace Code.Gameplay.Pickups.Queries
{
	public interface IScoreQuery
	{
		event Action<int> OnScoreChanged;
		int GetScore();
	}
}
