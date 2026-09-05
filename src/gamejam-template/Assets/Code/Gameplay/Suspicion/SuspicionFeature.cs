using Code.Gameplay.Suspicion.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Suspicion
{
	public sealed class SuspicionFeature : Feature
	{
		public SuspicionFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<AccumulateSuspicionWhileWatchedSystem>());
			Add(systemFactory.Create<DecaySuspicionSystem>());

			Add(systemFactory.Create<AddSuspicionOnWrongInputSystem>());
			Add(systemFactory.Create<AddSuspicionOnMeowWhileWatchedSystem>());

			Add(systemFactory.Create<FinishExamOnMaxSuspicionSystem>());
		}
	}
}
