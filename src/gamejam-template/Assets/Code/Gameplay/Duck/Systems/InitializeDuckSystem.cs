using Code.Gameplay.Duck.Services;
using Entitas;

namespace Code.Gameplay.Duck.Systems
{
	public class InitializeDuckSystem : IInitializeSystem
	{
		private readonly IDuckFactory _duckFactory;

		private readonly IGroup<GameEntity> _ducks;

		public InitializeDuckSystem(GameContext game, IDuckFactory duckFactory)
		{
			_duckFactory = duckFactory;

			_ducks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Duck));
		}

		public void Initialize()
		{
			if (_ducks.count > 0)
				return;

			_duckFactory.CreateDuck();
		}
	}
}
