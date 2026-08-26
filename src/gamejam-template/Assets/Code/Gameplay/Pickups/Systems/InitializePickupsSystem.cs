using Code.Gameplay.Pickups.Configs;
using Code.Gameplay.Pickups.Services;
using Code.Infrastructure.Randomization;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Pickups.Systems
{
	public class InitializePickupsSystem : IInitializeSystem
	{
		private readonly IPickupFactory _pickupFactory;
		private readonly IPickupConfigsService _pickupConfigsService;
		private readonly IRandomService _randomService;

		private readonly IGroup<GameEntity> _spawnPoints;

		public InitializePickupsSystem(
			GameContext game,
			IPickupFactory pickupFactory,
			IPickupConfigsService pickupConfigsService,
			IRandomService randomService)
		{
			_pickupFactory = pickupFactory;
			_pickupConfigsService = pickupConfigsService;
			_randomService = randomService;

			_spawnPoints = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.SpawnPoint,
					GameMatcher.WorldPosition));
		}

		public void Initialize()
		{
			PickupsConfig config = _pickupConfigsService.PickupsConfig;
			Vector3 center = GetSpawnCenter();

			for (int index = 0; index < config.SpawnCount; index++)
			{
				_pickupFactory.CreatePickup(GetPickupPosition(center, config));
			}
		}

		private Vector3 GetSpawnCenter()
		{
			foreach (GameEntity spawnPoint in _spawnPoints)
				return spawnPoint.WorldPosition;

			return Vector3.zero;
		}

		private Vector3 GetPickupPosition(Vector3 center, PickupsConfig config)
		{
			float angle = _randomService.Range(0f, Mathf.PI * 2f);
			float distance = _randomService.Range(config.MinDistanceFromSpawn, config.SpawnRadius);

			return center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * distance;
		}
	}
}
