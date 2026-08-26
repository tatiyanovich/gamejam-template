using Code.Gameplay.Movement.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Movement
{
	public sealed class MovementFixedUpdateFeature : Feature
	{
		public MovementFixedUpdateFeature(ISystemFactory systems)
		{
			Add(systems.Create<AllowMovementSystem>());

			Add(systems.Create<ForbidMovementOnDeathSystem>());

			Add(systems.Create<ApplyImpulseSystem>());

			Add(systems.Create<RigidbodyPositionMovementSystem>());
			Add(systems.Create<RigidbodyVelocityMovementSystem>());

			Add(systems.Create<RigidbodyInterpolationMovementSystem>());

			Add(systems.Create<RefreshRigidbodyWithVelocitySystem>());
			Add(systems.Create<RefreshVelocityWithRigidbodySystem>());
		}
	}
}