using Code.Common.Extensions;
using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;

namespace Code.Storage.Systems
{
	public class AutoSaveSystem : IExecuteSystem
	{
		private readonly IEntityFactory _entityFactory;
		private readonly int _autoSaveInterval;
		private readonly IGroup<GameEntity> _autoSaveTimer;
		private readonly IGroup<GameEntity> _autoSaveIntervalsUp;

		public AutoSaveSystem(
			GameContext game,
			IEntityFactory entityFactory,
			int autoSaveInterval)
		{
			_entityFactory = entityFactory;
			_autoSaveInterval = autoSaveInterval;

			_autoSaveTimer = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.AutoSaveTimer,
					GameMatcher.Timer,
					GameMatcher.Interval));

			_autoSaveIntervalsUp = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.AutoSaveTimer,
					GameMatcher.Timer,
					GameMatcher.Interval,
					GameMatcher.IntervalUp));
		}

		public void Execute()
		{
			if (_autoSaveTimer.count == 0)
				_entityFactory.Game()
					.With(x => x.isAutoSaveTimer = true)
					.With(x => x.isTimer = true)
					.AddInterval(_autoSaveInterval)
					.AddTimeLeft(_autoSaveInterval);

			if (_autoSaveIntervalsUp.count > 0)
				_entityFactory.Request().isSaveProgressRequest = true;
		}
	}
}
