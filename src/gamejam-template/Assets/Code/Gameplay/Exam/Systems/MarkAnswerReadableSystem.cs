using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public class MarkAnswerReadableSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _runningExams;
		private readonly IGroup<GameEntity> _questions;
		private readonly IGroup<InputEntity> _leaningInputs;

		private readonly List<GameEntity> _buffer = new(1);

		public MarkAnswerReadableSystem(GameContext game, InputContext input)
		{
			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun)
				.NoneOf(
					GameMatcher.ExamFinished));

			_questions = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question));

			_leaningInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.LeanHeld));
		}

		public void Execute()
		{
			bool readable = _runningExams.count > 0 && _leaningInputs.count > 0;

			foreach (GameEntity question in _questions.GetEntities(_buffer))
			{
				question.isAnswerReadable = readable;
			}
		}
	}
}
