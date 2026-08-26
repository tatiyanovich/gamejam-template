using System.Collections.Generic;
using Code.Gameplay.CoreLoop;
using Code.Infrastructure.AppMetadata.Snapshots;
using Code.Infrastructure.EntityComponentSystem.SceneEntities.Snapshots;
using Framework.Storage;

namespace Code.Storage.SaveFiles
{
	// The whole player progress, serialized as JSON. Add a field per feature that must survive a
	// restart, keep it a plain snapshot DTO, and refresh it from a system in RefreshSnapshotsFeature.
	public class GeneralSaveFile : ISaveFile
	{
		public int SchemeVersion;
		public AppMetadataSnapshot AppMetadata;
		public SessionLoopSnapshot SessionLoop;
		public List<SceneEntitySnapshot> SceneEntities = new();
		public int Score;
	}
}
