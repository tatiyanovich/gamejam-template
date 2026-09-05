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
					GameMatcher.PlayerName));
		}

		public string GetPlayerName()
		{
			foreach (GameEntity progress in _progresses)
				return progress.PlayerName;

			return string.Empty;
		}
	}
}
