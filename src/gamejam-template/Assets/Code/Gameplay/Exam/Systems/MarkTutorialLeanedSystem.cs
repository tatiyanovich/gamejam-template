using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public sealed class MarkTutorialLeanedSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _runs;
		private readonly IGroup<InputEntity> _leaningInputs;

		private readonly List<GameEntity> _buffer = new(1);

		public MarkTutorialLeanedSystem(GameContext game, InputContext input)
		{
			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun)
				.NoneOf(
					GameMatcher.TutorialLeaned));

			_leaningInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.LeanHeld));
		}

		public void Execute()
		{
			if (_leaningInputs.count == 0)
				return;

			foreach (GameEntity run in _runs.GetEntities(_buffer))
				run.isTutorialLeaned = true;
		}
	}
}
