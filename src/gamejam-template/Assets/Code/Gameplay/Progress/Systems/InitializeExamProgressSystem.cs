using Code.Gameplay.Progress.Services;
using Code.Storage.SaveFiles;
using Entitas;
using Framework.Storage;

namespace Code.Gameplay.Progress.Systems
{
	public class InitializeExamProgressSystem : IInitializeSystem
	{
		private readonly ISaveLoadService _saveLoadService;
		private readonly IProgressFactory _progressFactory;

		private readonly IGroup<GameEntity> _progresses;

		public InitializeExamProgressSystem(
			GameContext game,
			ISaveLoadService saveLoadService,
			IProgressFactory progressFactory)
		{
			_saveLoadService = saveLoadService;
			_progressFactory = progressFactory;

			_progresses = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamProgress));
		}

		public void Initialize()
		{
			if (_progresses.count > 0)
				return;

			_progressFactory.CreateExamProgress(_saveLoadService.Get<GeneralSaveFile>());
		}
	}
}
