using Code.Gameplay.Movement;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay
{
	public sealed class FixedUpdateFeature : Feature
	{
		public FixedUpdateFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<MovementFixedUpdateFeature>());
		}
	}
}
