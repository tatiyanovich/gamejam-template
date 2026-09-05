using Entitas;

namespace Code.Gameplay.Progress.Queries
{
	public sealed class ProgressQuery : IProgressQuery
	{
		private readonly IGroup<GameEntity> _progresses;

		public ProgressQuery(GameContext game)
		{
			_progresses = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamProgress,
					GameMatcher.PlayerName,
					GameMatcher.BestAnswers,
					GameMatcher.BestTimeSeconds));
		}

		public string GetPlayerName()
		{
			foreach (GameEntity progress in _progresses)
				return progress.PlayerName;

			return string.Empty;
		}

		public int GetBestAnswers()
		{
			foreach (GameEntity progress in _progresses)
				return progress.BestAnswers;

			return 0;
		}

		public float GetBestTimeSeconds()
		{
			foreach (GameEntity progress in _progresses)
				return progress.BestTimeSeconds;

			return 0f;
		}
	}
}
