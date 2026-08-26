using Code.Gameplay.Camera.Services;
using Code.Gameplay.Player.Services;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Player.Systems
{
	public class InitializePlayerSystem : IInitializeSystem
	{
		private readonly IPlayerFactory _playerFactory;
		private readonly ICameraFactory _cameraFactory;
		private readonly IGroup<GameEntity> _spawnPoints;

		public InitializePlayerSystem(
			GameContext game,
			IPlayerFactory playerFactory,
			ICameraFactory cameraFactory)
		{
			_playerFactory = playerFactory;
			_cameraFactory = cameraFactory;
			_spawnPoints = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.SpawnPoint,
					GameMatcher.WorldPosition));
		}

		public void Initialize()
		{
			Vector3 spawnPosition = GetSpawnPosition();

			GameEntity player = _playerFactory.CreatePlayer(spawnPosition);
			_cameraFactory.CreateCamera(player.Id, spawnPosition);
		}

		private Vector3 GetSpawnPosition()
		{
			foreach (GameEntity spawnPoint in _spawnPoints)
				return spawnPoint.WorldPosition;

			Debug.LogWarning("No SpawnPoint found in the scene. Spawning the player at world origin.");
			return Vector3.zero;
		}
	}
}
