using System.Collections.Generic;
using Entitas;
using Framework.Essentials.TimeManagement;

namespace Code.Gameplay.Neighbours.Systems
{
	public class CoverPawOnTimerSystem : IExecuteSystem
	{
		private readonly ITimeService _timeService;

		private readonly IGroup<GameEntity> _liftedPaws;

		private readonly List<GameEntity> _buffer = new(2);

		public CoverPawOnTimerSystem(GameContext game, ITimeService timeService)
		{
			_timeService = timeService;

			_liftedPaws = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Neighbour,
					GameMatcher.PawLifted,
					GameMatcher.PawWindowTimeLeft));
		}

		public void Execute()
		{
			foreach (GameEntity neighbour in _liftedPaws.GetEntities(_buffer))
			{
				neighbour.ReplacePawWindowTimeLeft(neighbour.PawWindowTimeLeft - _timeService.DeltaTime);

				if (neighbour.PawWindowTimeLeft > 0)
					continue;

				neighbour.ReplacePawWindowTimeLeft(0f);
				neighbour.isPawLifted = false;
			}
		}
	}
}
