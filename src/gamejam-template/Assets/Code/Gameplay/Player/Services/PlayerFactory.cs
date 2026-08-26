using Code.Gameplay.Player.Configs;
using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;
using UnityEngine;

namespace Code.Gameplay.Player.Services
{
	public class PlayerFactory : IPlayerFactory
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IIdentifierService _identifierService;
		private readonly IPlayerConfigsService _playerConfigsService;

		public PlayerFactory(
			IEntityFactory entityFactory,
			IIdentifierService identifierService,
			IPlayerConfigsService playerConfigsService)
		{
			_entityFactory = entityFactory;
			_identifierService = identifierService;
			_playerConfigsService = playerConfigsService;
		}

		public GameEntity CreatePlayer(Vector3 at)
		{
			PlayerConfig playerConfig = _playerConfigsService.PlayerConfig;

			return _entityFactory.Game()
				.AddId(_identifierService.Next())
				.With(x => x.isPlayer = true)
				.With(x => x.isCanMove = true)
				.With(x => x.isKinematicMovement = true)

				.AddWorldPosition(at)
				.AddPreviousWorldPosition(at)
				.AddMovementDirection(Vector3.zero)
				.AddVelocity(Vector3.zero)
				.AddTargetVelocity(Vector3.zero)

				.AddMovementSpeed(playerConfig.MoveSpeed)
				.AddMaxMovementSpeed(playerConfig.MaxMoveSpeed)
				.AddMovementAcceleration(playerConfig.MoveAcceleration)
				.AddMovementSpeedMultiplier(1f)
				.AddRotationSpeedMultiplier(1f)
				.AddRotationSharpness(playerConfig.RotationSharpness)
				.AddWorldRotation(Quaternion.identity)
				.AddLookDirection(Vector3.up)

				.AddViewAddressableKey(Addresses.PlayerCharacterKey);
		}
	}
}
