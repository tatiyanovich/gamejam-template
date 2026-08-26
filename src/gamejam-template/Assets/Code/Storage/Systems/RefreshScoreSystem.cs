using Code.Storage.SaveFiles;
using Entitas;
using Framework.Storage;

namespace Code.Storage.Systems
{
	public class RefreshScoreSystem : IExecuteSystem
	{
		private readonly ISaveLoadService _saveLoadService;

		private readonly IGroup<GameEntity> _scoreHolders;

		public RefreshScoreSystem(GameContext game, ISaveLoadService saveLoadService)
		{
			_saveLoadService = saveLoadService;

			_scoreHolders = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ScoreHolder,
					GameMatcher.Score));
		}

		public void Execute()
		{
			GeneralSaveFile saveFile = _saveLoadService.Get<GeneralSaveFile>();

			foreach (GameEntity scoreHolder in _scoreHolders)
			{
				saveFile.Score = scoreHolder.Score;
			}
		}
	}
}
