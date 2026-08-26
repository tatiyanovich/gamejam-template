using Code.Gameplay.Pickups.Services;
using Code.Storage.SaveFiles;
using Entitas;
using Framework.Storage;

namespace Code.Gameplay.Pickups.Systems
{
	// Restores the saved score into an entity when the gameplay node starts. Saveable state always
	// enters the world through its snapshot — never by mutating the save file from gameplay.
	public class InitializeScoreSystem : IInitializeSystem
	{
		private readonly IPickupFactory _pickupFactory;
		private readonly ISaveLoadService _saveLoadService;

		private readonly IGroup<GameEntity> _scoreHolders;

		public InitializeScoreSystem(
			GameContext game,
			IPickupFactory pickupFactory,
			ISaveLoadService saveLoadService)
		{
			_pickupFactory = pickupFactory;
			_saveLoadService = saveLoadService;

			_scoreHolders = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ScoreHolder));
		}

		public void Initialize()
		{
			if (_scoreHolders.count > 0)
				return;

			_pickupFactory.CreateScoreHolder(_saveLoadService.Get<GeneralSaveFile>().Score);
		}
	}
}
