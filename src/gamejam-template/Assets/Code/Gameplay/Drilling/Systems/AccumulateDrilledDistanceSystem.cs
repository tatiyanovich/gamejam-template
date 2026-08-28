using Entitas;
using Framework.Essentials.TimeManagement;
using UnityEngine;

namespace Code.Gameplay.Drilling.Systems
{
	public class AccumulateDrilledDistanceSystem : IExecuteSystem
	{
		private readonly ITimeService _timeService;

		private readonly IGroup<GameEntity> _movingDrills;
		private readonly IGroup<GameEntity> _runs;

		public AccumulateDrilledDistanceSystem(GameContext game, ITimeService timeService)
		{
			_timeService = timeService;

			_movingDrills = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Player,
					GameMatcher.Velocity,
					GameMatcher.Moving,
					GameMatcher.CanMove));

			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.DrillRun,
					GameMatcher.DrilledDistance));
		}

		public void Execute()
		{
			float deltaTime = _timeService.DeltaTime;

			foreach (GameEntity drill in _movingDrills)
			foreach (GameEntity run in _runs)
			{
				float travelled = drill.Velocity.magnitude * deltaTime;

				run.ReplaceDrilledDistance(run.DrilledDistance + travelled);
			}
		}
	}
}
