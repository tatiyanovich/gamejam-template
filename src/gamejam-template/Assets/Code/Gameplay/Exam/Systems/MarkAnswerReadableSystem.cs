using System.Collections.Generic;
using Code.Gameplay.Neighbours;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public class MarkAnswerReadableSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _runningExams;
		private readonly IGroup<GameEntity> _questions;
		private readonly IGroup<GameEntity> _liftedNeighbours;
		private readonly IGroup<InputEntity> _leaningInputs;

		private readonly List<GameEntity> _buffer = new(1);
		private readonly List<GameEntity> _neighbourBuffer = new(2);

		public MarkAnswerReadableSystem(GameContext game, InputContext input)
		{
			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun)
				.NoneOf(
					GameMatcher.ExamFinished));

			_questions = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question,
					GameMatcher.AnswerNeighbourSide));

			_liftedNeighbours = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Neighbour,
					GameMatcher.NeighbourSide,
					GameMatcher.PawLifted));

			_leaningInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.LeanHeld));
		}

		public void Execute()
		{
			bool leaning = _runningExams.count > 0 && _leaningInputs.count > 0;

			foreach (GameEntity question in _questions.GetEntities(_buffer))
			{
				question.isAnswerReadable = leaning && IsPawLifted(question.AnswerNeighbourSide);
			}
		}

		private bool IsPawLifted(NeighbourSide side)
		{
			foreach (GameEntity neighbour in _liftedNeighbours.GetEntities(_neighbourBuffer))
			{
				if (neighbour.NeighbourSide == side)
					return true;
			}

			return false;
		}
	}
}
