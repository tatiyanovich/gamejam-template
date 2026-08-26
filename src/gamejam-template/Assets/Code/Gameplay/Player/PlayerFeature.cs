using Code.Gameplay.Movement.Systems;
using Code.Gameplay.Player.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Player
{
	public sealed class PlayerFeature : Feature
	{
		public PlayerFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<InitializePlayerSystem>());

			Add(systemFactory.Create<ResetMovementSpeedMultiplierSystem>());
			Add(systemFactory.Create<ResetRotationSpeedMultiplierSystem>());
			Add(systemFactory.Create<ResetPlayerAccelerationSystem>());

			Add(systemFactory.Create<SetPlayerMoveDirectionSystem>());
			Add(systemFactory.Create<SetPlayerLookDirectionSystem>());

			Add(systemFactory.Create<ComputePlayerTargetVelocitySystem>());
			Add(systemFactory.Create<AccelerateTowardsTargetVelocitySystem>());
			Add(systemFactory.Create<IntegrateKinematicVelocitySystem>());
			Add(systemFactory.Create<ResolveKinematicCollisionSystem>());
			Add(systemFactory.Create<SyncKinematicTransformSystem>());
		}
	}
}
