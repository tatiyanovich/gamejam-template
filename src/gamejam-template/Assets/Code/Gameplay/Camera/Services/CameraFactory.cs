using Code.Gameplay.Camera.Configs;
using Code.Gameplay.Movement;
using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;
using UnityEngine;

namespace Code.Gameplay.Camera.Services
{
	public class CameraFactory : ICameraFactory
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IIdentifierService _identifierService;
		private readonly ICameraConfigsService _cameraConfigsService;

		public CameraFactory(
			IEntityFactory entityFactory,
			IIdentifierService identifierService,
			ICameraConfigsService cameraConfigsService)
		{
			_identifierService = identifierService;
			_entityFactory = entityFactory;
			_cameraConfigsService = cameraConfigsService;
		}

		public void CreateShakeRequest(CameraShakeTypeId shakeTypeId, float scale = 1f)
		{
			_entityFactory.Request()
				.AddCameraShakeRequest(shakeTypeId)
				.AddCameraShakeScale(scale);
		}

		public GameEntity CreateCamera(int playerId, Vector3 position)
		{
			CameraConfig config = _cameraConfigsService.PlayerCameraConfig;

			return _entityFactory.Game()
				.AddId(_identifierService.Next())
				.AddTargetId(playerId)

				.AddViewAddressableKey(Addresses.CameraPrefab)
				.AddWorldRotation(Quaternion.Euler(config.Rotation))

				.SetupSmoothFollowMovement(
					worldPosition: position + config.Offset,
					followOffset: config.Offset,
					followSmoothSpeed: config.Smoothing);
		}

		public GameEntity CreateStaticCamera(Vector3 position)
		{
			CameraConfig config = _cameraConfigsService.PlayerCameraConfig;

			return _entityFactory.Game()
				.AddId(_identifierService.Next())

				.AddViewAddressableKey(Addresses.CameraPrefab)
				.AddWorldPosition(position + config.Offset)
				.AddWorldRotation(Quaternion.Euler(config.Rotation));
		}
	}
}
