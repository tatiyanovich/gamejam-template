using System.Collections.Generic;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public class ValidateWordInputSystem : IExecuteSystem
	{
		private readonly IEntityFactory _entityFactory;

		private readonly IGroup<GameEntity> _wordQuestions;
		private readonly IGroup<InputEntity> _letterInputs;

		private readonly List<GameEntity> _buffer = new(1);

		public ValidateWordInputSystem(
			GameContext game,
			InputContext input,
			IEntityFactory entityFactory)
		{
			_entityFactory = entityFactory;

			_wordQuestions = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question,
					GameMatcher.QuestionIndex,
					GameMatcher.AnswerReadable,
					GameMatcher.AnswerWord,
					GameMatcher.AnswerProgress)
				.NoneOf(
					GameMatcher.AnswerCopied));

			_letterInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.LetterInput));
		}

		public void Execute()
		{
			foreach (InputEntity input in _letterInputs)
			{
				foreach (GameEntity question in _wordQuestions.GetEntities(_buffer))
				{
					Validate(question, input.LetterInput);
				}
			}
		}

		private void Validate(GameEntity question, char letter)
		{
			if (char.ToUpperInvariant(question.AnswerWord[question.AnswerProgress]) == letter)
			{
				question.ReplaceAnswerProgress(question.AnswerProgress + 1);
				return;
			}

			_entityFactory.Event()
				.AddWrongInputEvent(question.QuestionIndex);
		}
	}
}
