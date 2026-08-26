using Code.Gameplay.Movement.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Movement
{
	public sealed class MovementLateUpdateFeature : Feature
	{
		public MovementLateUpdateFeature(ISystemFactory systems)
		{
			Add(systems.Create<UpdateTransformWorldPositionLateSystem>());

			Add(systems.Create<SmoothFollowMovementSystem>());
			Add(systems.Create<UpdateSmoothFollowTransformSystem>());
		}
	}
}
