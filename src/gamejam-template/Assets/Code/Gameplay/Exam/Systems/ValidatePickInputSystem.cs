using System.Collections.Generic;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public class ValidatePickInputSystem : IExecuteSystem
	{
		private readonly IEntityFactory _entityFactory;

		private readonly IGroup<GameEntity> _pickQuestions;
		private readonly IGroup<InputEntity> _pickInputs;

		private readonly List<GameEntity> _buffer = new(1);

		public ValidatePickInputSystem(
			GameContext game,
			InputContext input,
			IEntityFactory entityFactory)
		{
			_entityFactory = entityFactory;

			_pickQuestions = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question,
					GameMatcher.QuestionIndex,
					GameMatcher.AnswerReadable,
					GameMatcher.AnswerOptions,
					GameMatcher.CorrectOptionIndex,
					GameMatcher.AnswerProgress)
				.NoneOf(
					GameMatcher.AnswerCopied));

			_pickInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.PickInput));
		}

		public void Execute()
		{
			foreach (InputEntity input in _pickInputs)
			{
				foreach (GameEntity question in _pickQuestions.GetEntities(_buffer))
				{
					Validate(question, input.PickInput);
				}
			}
		}

		private void Validate(GameEntity question, int optionIndex)
		{
			if (question.CorrectOptionIndex == optionIndex)
			{
				question.ReplaceAnswerProgress(question.AnswerProgress + 1);
				return;
			}

			_entityFactory.Event()
				.AddWrongInputEvent(question.QuestionIndex);
		}
	}
}
