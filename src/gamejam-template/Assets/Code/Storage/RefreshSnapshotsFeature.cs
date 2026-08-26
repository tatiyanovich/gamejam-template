using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Storage.Systems;

namespace Code.Storage
{
	// Runs only when ISaveLoadService asks for a save. Systems never write the save file during
	// gameplay — they mutate entities, and these systems copy entity state into snapshots.
	public sealed class RefreshSnapshotsFeature : Feature
	{
		public RefreshSnapshotsFeature(ISystemFactory systems)
		{
			Add(systems.Create<RefreshAppMetadataSystem>());
			Add(systems.Create<RefreshScoreSystem>());
		}
	}
}
