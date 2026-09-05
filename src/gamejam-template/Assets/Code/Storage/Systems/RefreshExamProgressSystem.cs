using Code.Storage.SaveFiles;
using Entitas;
using Framework.Storage;

namespace Code.Storage.Systems
{
	public class RefreshExamProgressSystem : IExecuteSystem
	{
		private readonly ISaveLoadService _saveLoadService;

		private readonly IGroup<GameEntity> _progresses;

		public RefreshExamProgressSystem(GameContext game, ISaveLoadService saveLoadService)
		{
			_saveLoadService = saveLoadService;

			_progresses = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamProgress,
					GameMatcher.PlayerName,
					GameMatcher.BestAnswers,
					GameMatcher.BestTimeSeconds));
		}

		public void Execute()
		{
			GeneralSaveFile saveFile = _saveLoadService.Get<GeneralSaveFile>();

			foreach (GameEntity progress in _progresses)
			{
				saveFile.PlayerName = progress.PlayerName;
				saveFile.IntroSeen = progress.isIntroSeen;
				saveFile.BestAnswers = progress.BestAnswers;
				saveFile.BestTimeSeconds = progress.BestTimeSeconds;
			}
		}
	}
}
