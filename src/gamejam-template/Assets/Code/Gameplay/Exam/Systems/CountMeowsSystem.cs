using System.Collections.Generic;
using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public class CountMeowsSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _runs;
		private readonly IGroup<GameEntity> _meows;

		private readonly List<GameEntity> _buffer = new(1);

		public CountMeowsSystem(GameContext game)
		{
			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.MeowCount)
				.NoneOf(
					GameMatcher.ExamFinished));

			_meows = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.MeowEvent));
		}

		public void Execute()
		{
			if (_meows.count == 0)
				return;

			foreach (GameEntity run in _runs.GetEntities(_buffer))
			{
				run.ReplaceMeowCount(run.MeowCount + _meows.count);
			}
		}
	}
}
