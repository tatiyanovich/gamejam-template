using System.Collections.Generic;
using Code.Gameplay.Exam.Services;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public class MarkAnswerCopiedSystem : IExecuteSystem
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IExamConfigsService _examConfigsService;

		private readonly IGroup<GameEntity> _answeredQuestions;

		private readonly List<GameEntity> _buffer = new(1);

		public MarkAnswerCopiedSystem(
			GameContext game,
			IEntityFactory entityFactory,
			IExamConfigsService examConfigsService)
		{
			_entityFactory = entityFactory;
			_examConfigsService = examConfigsService;

			_answeredQuestions = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question,
					GameMatcher.QuestionIndex,
					GameMatcher.AnswerProgress,
					GameMatcher.AnswerLength)
				.NoneOf(
					GameMatcher.AnswerCopied));
		}

		public void Execute()
		{
			foreach (GameEntity question in _answeredQuestions.GetEntities(_buffer))
			{
				if (question.AnswerProgress < question.AnswerLength)
					continue;

				question.isAnswerCopied = true;
				question.AddLifetimeLeft(_examConfigsService.ExamConfig.QuestionPauseSeconds);

				_entityFactory.Event()
					.AddAnswerCopiedEvent(question.QuestionIndex);
			}
		}
	}
}
