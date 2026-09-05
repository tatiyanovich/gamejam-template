using System.Collections.Generic;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public class ValidateStrokeInputSystem : IExecuteSystem
	{
		private readonly IEntityFactory _entityFactory;

		private readonly IGroup<GameEntity> _strokeQuestions;
		private readonly IGroup<InputEntity> _strokeInputs;

		private readonly List<GameEntity> _buffer = new(1);

		public ValidateStrokeInputSystem(
			GameContext game,
			InputContext input,
			IEntityFactory entityFactory)
		{
			_entityFactory = entityFactory;

			_strokeQuestions = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question,
					GameMatcher.QuestionIndex,
					GameMatcher.AnswerReadable,
					GameMatcher.AnswerStrokes,
					GameMatcher.AnswerProgress)
				.NoneOf(
					GameMatcher.AnswerCopied));

			_strokeInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.StrokeInput));
		}

		public void Execute()
		{
			foreach (InputEntity input in _strokeInputs)
			{
				foreach (GameEntity question in _strokeQuestions.GetEntities(_buffer))
				{
					Validate(question, input.StrokeInput);
				}
			}
		}

		private void Validate(GameEntity question, StrokeDirection stroke)
		{
			if (question.AnswerStrokes[question.AnswerProgress] == stroke)
			{
				question.ReplaceAnswerProgress(question.AnswerProgress + 1);
				return;
			}

			_entityFactory.Event()
				.AddWrongInputEvent(question.QuestionIndex);
		}
	}
}
