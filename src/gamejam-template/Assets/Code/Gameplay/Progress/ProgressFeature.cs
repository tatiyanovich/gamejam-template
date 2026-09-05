using Code.Gameplay.Progress.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Progress
{
	public sealed class ProgressFeature : Feature
	{
		public ProgressFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<InitializeExamProgressSystem>());

			Add(systemFactory.Create<SetPlayerNameByRequestSystem>());
			Add(systemFactory.Create<MarkIntroSeenByRequestSystem>());

			Add(systemFactory.Create<RecordBestExamResultSystem>());
		}
	}
}
