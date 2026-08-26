using Entitas;
using Framework.Essentials.TimeManagement;

namespace Code.Gameplay.Lifetime.Systems
{
	public class LifetimeCounterSystem : IExecuteSystem
	{
		private readonly ITimeService _timeService;
		private readonly IGroup<GameEntity> _entities;

		public LifetimeCounterSystem(GameContext game, ITimeService timeService)
		{
			_timeService = timeService;
			_entities = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.LifetimeLeft));
		}

		public void Execute()
		{
			foreach (GameEntity entity in _entities)
			{
				entity.ReplaceLifetimeLeft(entity.LifetimeLeft - _timeService.DeltaTime);
			}
		}
	}
}
