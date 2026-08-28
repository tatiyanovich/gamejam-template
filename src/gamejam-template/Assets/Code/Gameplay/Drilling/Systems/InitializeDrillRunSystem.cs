using Code.Gameplay.Drilling.Services;
using Code.Storage.SaveFiles;
using Entitas;
using Framework.Storage;

namespace Code.Gameplay.Drilling.Systems
{
	public class InitializeDrillRunSystem : IInitializeSystem
	{
		private readonly IDrillRunFactory _drillRunFactory;
		private readonly ISaveLoadService _saveLoadService;

		private readonly IGroup<GameEntity> _runs;

		public InitializeDrillRunSystem(
			GameContext game,
			IDrillRunFactory drillRunFactory,
			ISaveLoadService saveLoadService)
		{
			_drillRunFactory = drillRunFactory;
			_saveLoadService = saveLoadService;

			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.DrillRun));
		}

		public void Initialize()
		{
			if (_runs.count > 0)
				return;

			_drillRunFactory.CreateRun(_saveLoadService.Get<GeneralSaveFile>().BestDrilledDistance);
		}
	}
}
