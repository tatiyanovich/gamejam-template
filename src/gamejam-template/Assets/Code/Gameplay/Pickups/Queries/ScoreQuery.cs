using System;
using Code.Infrastructure.EntityComponentSystem;
using Entitas;

namespace Code.Gameplay.Pickups.Queries
{
	// The read side of the score for Views: they subscribe instead of polling, and
	// NotifyQueryChangesSystem drives ReactToChanges once per frame after gameplay.
	public class ScoreQuery : IScoreQuery, IReactiveQuery
	{
		private readonly IGroup<GameEntity> _scoreHolders;
		private readonly IGroup<GameEntity> _changedScoreHolders;

		public event Action<int> OnScoreChanged;

		public ScoreQuery(GameContext game)
		{
			_scoreHolders = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ScoreHolder,
					GameMatcher.Score));

			_changedScoreHolders = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ScoreHolder,
					GameMatcher.Score,
					GameMatcher.ScoreChanged));
		}

		public void ReactToChanges()
		{
			foreach (GameEntity scoreHolder in _changedScoreHolders)
			{
				OnScoreChanged?.Invoke(scoreHolder.Score);
			}
		}

		public int GetScore()
		{
			foreach (GameEntity scoreHolder in _scoreHolders)
				return scoreHolder.Score;

			return 0;
		}
	}
}
