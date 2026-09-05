using System.Collections.Generic;
using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public sealed class MarkTutorialDuckThrownSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _runs;
		private readonly IGroup<GameEntity> _duckThrows;

		private readonly List<GameEntity> _buffer = new(1);

		public MarkTutorialDuckThrownSystem(GameContext game)
		{
			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun)
				.NoneOf(
					GameMatcher.TutorialDuckThrown));

			_duckThrows = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.DuckThrownEvent));
		}

		public void Execute()
		{
			if (_duckThrows.count == 0)
				return;

			foreach (GameEntity run in _runs.GetEntities(_buffer))
				run.isTutorialDuckThrown = true;
		}
	}
}
