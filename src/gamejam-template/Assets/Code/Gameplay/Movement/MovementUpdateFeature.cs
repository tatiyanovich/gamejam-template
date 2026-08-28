using Code.Gameplay.Movement.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Movement
{
	public sealed class MovementUpdateFeature : Feature
	{
		public MovementUpdateFeature(ISystemFactory systems)
		{
			Add(systems.Create<ComputeMovementStepSystem>());
			
			Add(systems.Create<MarkIsMovingSystem>());
			Add(systems.Create<TransformMovementSystem>());

			Add(systems.Create<RotateTowardsLookDirectionSystem>());
			Add(systems.Create<RotateTowardsLookDirection2DSystem>());

			Add(systems.Create<UpdateWorldPositionForRigidbodiesSystem>());
			Add(systems.Create<UpdateTransformWorldPositionSystem>());
			Add(systems.Create<UpdateTransformLocalPositionSystem>());
			Add(systems.Create<UpdateTransformRotationSystem>());
			Add(systems.Create<UpdateRigidbodyRotationSystem>());
			Add(systems.Create<UpdateTransformScaleSystem>());
		}
	}
}
