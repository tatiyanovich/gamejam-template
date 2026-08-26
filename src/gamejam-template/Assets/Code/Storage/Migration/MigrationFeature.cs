using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Storage.Migration
{
	public sealed class MigrationFeature : Feature
	{
		public MigrationFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<Migrate0InitializeSessionLoopStateSystem>());
		}
	}
}
