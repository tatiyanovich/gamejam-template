using Code.Gameplay.Input.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Input
{
	public sealed class InputFeature : Feature
	{
		public InputFeature(ISystemFactory systems)
		{
			Add(systems.Create<InitializeInputSystem>());
			Add(systems.Create<EmitInputSystem>());
		}
	}
}