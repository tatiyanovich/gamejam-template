using Code.Gameplay.Bell.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Bell
{
	public sealed class BellFeature : Feature
	{
		public BellFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<AnnounceBellSystem>());

			Add(systemFactory.Create<FinishExamOnBellSystem>());
		}
	}
}
