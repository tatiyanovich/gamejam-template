using Code.Gameplay.Greybox.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Greybox
{
	public sealed class GreyboxFeature : Feature
	{
		public GreyboxFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<InitializeGreyboxBoardSystem>());

			Add(systemFactory.Create<ShowTeacherInGreyboxSystem>());
			Add(systemFactory.Create<ShowSuspicionInGreyboxSystem>());
			Add(systemFactory.Create<ShowMeowInGreyboxSystem>());
			Add(systemFactory.Create<ShowNeighbourPawInGreyboxSystem>());

			Add(systemFactory.Create<ShowQuestionInGreyboxSystem>());
			Add(systemFactory.Create<ShowStrokeAnswerInGreyboxSystem>());
			Add(systemFactory.Create<ShowPickAnswerInGreyboxSystem>());
			Add(systemFactory.Create<ShowWordAnswerInGreyboxSystem>());

			Add(systemFactory.Create<ShowLeanInGreyboxSystem>());
			Add(systemFactory.Create<ShowExamProgressInGreyboxSystem>());
			Add(systemFactory.Create<ShowExamOutcomeInGreyboxSystem>());

			Add(systemFactory.Create<TearDownGreyboxBoardSystem>());
		}
	}
}
