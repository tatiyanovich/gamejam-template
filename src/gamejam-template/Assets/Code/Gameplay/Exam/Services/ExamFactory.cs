using Code.Gameplay.Exam.Data;
using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;

namespace Code.Gameplay.Exam.Services
{
	public class ExamFactory : IExamFactory
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IIdentifierService _identifierService;
		private readonly IExamConfigsService _examConfigsService;

		public ExamFactory(
			IEntityFactory entityFactory,
			IIdentifierService identifierService,
			IExamConfigsService examConfigsService)
		{
			_entityFactory = entityFactory;
			_identifierService = identifierService;
			_examConfigsService = examConfigsService;
		}

		public GameEntity CreateRun()
		{
			return _entityFactory.Game()
				.AddId(_identifierService.Next())
				.With(x => x.isExamRun = true)
				.AddCurrentQuestionIndex(0)
				.AddAnswersCopied(0)
				.AddExamElapsedSeconds(0f)
				.AddSuspicionLevel(0f)
				.AddExamOutcome(ExamOutcome.None);
		}

		public GameEntity CreateQuestion(int questionIndex)
		{
			QuestionDefinition definition = _examConfigsService.ExamConfig.Questions[questionIndex];

			return _entityFactory.Game()
				.AddId(_identifierService.Next())
				.With(x => x.isQuestion = true)
				.AddQuestionIndex(questionIndex)
				.AddQuestionText(definition.Text)
				.AddQuestionType(definition.Type)
				.AddAnswerNeighbourSide(definition.Neighbour)
				.AddAnswerLength(GetAnswerLength(definition))
				.AddAnswerProgress(0)
				.With(x => x.AddAnswerStrokes(definition.Strokes), definition.Type == QuestionType.Strokes)
				.With(x => x.AddAnswerOptions(definition.Options), definition.Type == QuestionType.Pick)
				.With(x => x.AddCorrectOptionIndex(definition.CorrectOptionIndex), definition.Type == QuestionType.Pick)
				.With(x => x.AddAnswerWord(definition.Word), definition.Type == QuestionType.Word);
		}

		private static int GetAnswerLength(QuestionDefinition definition)
		{
			return definition.Type switch
			{
				QuestionType.Strokes => definition.Strokes.Count,
				QuestionType.Word => definition.Word.Length,
				_ => 1
			};
		}
	}
}
