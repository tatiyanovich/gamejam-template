using Code.Storage.SaveFiles;
using Entitas;
using Framework.Storage;

namespace Code.Storage.Systems
{
	public class RefreshDrillRunSystem : IExecuteSystem
	{
		private readonly ISaveLoadService _saveLoadService;

		private readonly IGroup<GameEntity> _runs;

		public RefreshDrillRunSystem(GameContext game, ISaveLoadService saveLoadService)
		{
			_saveLoadService = saveLoadService;

			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.DrillRun,
					GameMatcher.BestDrilledDistance));
		}

		public void Execute()
		{
			GeneralSaveFile saveFile = _saveLoadService.Get<GeneralSaveFile>();

			foreach (GameEntity run in _runs)
			{
				saveFile.BestDrilledDistance = run.BestDrilledDistance;
			}
		}
	}
}
