using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;

namespace Code.Gameplay.Input.Systems
{
	public class InitializeInputSystem : IInitializeSystem
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IGroup<InputEntity> _inputs;

		public InitializeInputSystem(
			InputContext input,
			IEntityFactory entityFactory)
		{
			_entityFactory = entityFactory;
			
			_inputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input));
		}

		public void Initialize()
		{
			if(_inputs.count > 0)
				return;
			
			_entityFactory.Input();
		}
	}
}
