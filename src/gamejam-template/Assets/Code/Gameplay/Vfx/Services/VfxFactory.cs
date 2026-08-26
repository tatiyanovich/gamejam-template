using Code.Gameplay.Movement;
using Code.Gameplay.Vfx.Data;
using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;
using UnityEngine;

namespace Code.Gameplay.Vfx.Services
{
	public class VfxFactory : IVfxFactory
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IIdentifierService _identifierService;

		private const float FollowSmoothSpeed = 40f;

		public VfxFactory(IEntityFactory entityFactory, IIdentifierService identifierService)
		{
			_entityFactory = entityFactory;
			_identifierService = identifierService;
		}

		public GameEntity Create(VfxSpawnRequest request)
		{
			Vector3 position = request.Position;
			position.z = 0f;

			GameEntity vfx = _entityFactory.Game()
				.AddId(_identifierService.Next())
				.With(x => x.isVfx = true)

				.AddWorldPosition(position)
				.AddWorldRotation(request.Rotation)
				.With(x => x.AddLossyScale(request.Scale), when: request.OverrideScale)
				.With(x => x.AddVfxTargetRadius(request.TargetRadius), when: request.HasTargetRadius)

				.With(x => x
					.AddTargetId(request.FollowId)
					.SetupSmoothFollowMovement(position, Vector3.zero, FollowSmoothSpeed), when: request.FollowId != 0)

				.AddViewAssetReference(request.Asset);

			return vfx;
		}
	}
}
