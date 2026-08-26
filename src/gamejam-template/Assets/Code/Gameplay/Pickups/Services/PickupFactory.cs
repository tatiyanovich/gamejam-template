using Code.Gameplay.Pickups.Configs;
using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;
using UnityEngine;

namespace Code.Gameplay.Pickups.Services
{
	public class PickupFactory : IPickupFactory
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IIdentifierService _identifierService;
		private readonly IPickupConfigsService _pickupConfigsService;

		public PickupFactory(
			IEntityFactory entityFactory,
			IIdentifierService identifierService,
			IPickupConfigsService pickupConfigsService)
		{
			_entityFactory = entityFactory;
			_identifierService = identifierService;
			_pickupConfigsService = pickupConfigsService;
		}

		public GameEntity CreatePickup(Vector3 at)
		{
			PickupsConfig config = _pickupConfigsService.PickupsConfig;

			return _entityFactory.Game()
				.AddId(_identifierService.Next())
				.With(x => x.isPickup = true)
				.AddScoreValue(config.ScorePerPickup)
				.AddCollectRadius(config.CollectRadius)
				.AddWorldPosition(at)
				.AddViewAddressableKey(Addresses.PickupPrefab);
		}

		// The score lives on its own entity so any system can read or raise it through a matcher,
		// and RefreshScoreSystem can copy it into the save file without knowing who scored.
		public GameEntity CreateScoreHolder(int score)
		{
			return _entityFactory.Game()
				.AddId(_identifierService.Next())
				.With(x => x.isScoreHolder = true)
				.AddScore(score);
		}
	}
}
