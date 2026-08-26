using Code.Gameplay.Movement;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay
{
	public sealed class LateUpdateFeature : Feature
	{
		public LateUpdateFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<MovementLateUpdateFeature>());
		}
	}
}
