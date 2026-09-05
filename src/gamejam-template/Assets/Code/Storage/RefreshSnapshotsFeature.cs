using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Storage.Systems;

namespace Code.Storage
{
	public sealed class RefreshSnapshotsFeature : Feature
	{
		public RefreshSnapshotsFeature(ISystemFactory systems)
		{
			Add(systems.Create<RefreshAppMetadataSystem>());
		}
	}
}
