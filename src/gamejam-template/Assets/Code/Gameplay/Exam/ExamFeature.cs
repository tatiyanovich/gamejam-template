using Code.Gameplay.Exam.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Exam
{
	public sealed class ExamFeature : Feature
	{
		public ExamFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<InitializeExamRunSystem>());
			Add(systemFactory.Create<SpawnNextQuestionSystem>());

			Add(systemFactory.Create<AccumulateExamTimeSystem>());

			Add(systemFactory.Create<MarkAnswerReadableSystem>());
			Add(systemFactory.Create<ValidateStrokeInputSystem>());
			Add(systemFactory.Create<ValidatePickInputSystem>());
			Add(systemFactory.Create<ValidateWordInputSystem>());

			Add(systemFactory.Create<MarkAnswerCopiedSystem>());
			Add(systemFactory.Create<AdvanceExamRunSystem>());
			Add(systemFactory.Create<FinishExamOnLastAnswerSystem>());
		}
	}
}
