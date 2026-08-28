using System.Collections.Generic;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;

namespace Code.Gameplay.Drilling.Systems
{
	public class FinishRunOnEmptyFuelSystem : IExecuteSystem
	{
		private readonly IEntityFactory _entityFactory;

		private readonly IGroup<GameEntity> _emptyDrills;
		private readonly IGroup<GameEntity> _unfinishedRuns;

		private readonly List<GameEntity> _buffer = new(1);

		public FinishRunOnEmptyFuelSystem(GameContext game, IEntityFactory entityFactory)
		{
			_entityFactory = entityFactory;

			_emptyDrills = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Player,
					GameMatcher.FuelEmpty));

			_unfinishedRuns = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.DrillRun)
				.NoneOf(
					GameMatcher.RunFinished));
		}

		public void Execute()
		{
			if (_emptyDrills.count == 0)
				return;

			foreach (GameEntity run in _unfinishedRuns.GetEntities(_buffer))
			{
				run.isRunFinished = true;

				_entityFactory.Request().isSaveProgressRequest = true;
			}
		}
	}
}
