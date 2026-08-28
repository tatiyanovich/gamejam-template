using System.Collections.Generic;
using Code.Gameplay.CoreLoop;
using Code.Infrastructure.AppMetadata.Snapshots;
using Code.Infrastructure.EntityComponentSystem.SceneEntities.Snapshots;
using Framework.Storage;

namespace Code.Storage.SaveFiles
{
	public class GeneralSaveFile : ISaveFile
	{
		public int SchemeVersion;
		public AppMetadataSnapshot AppMetadata;
		public SessionLoopSnapshot SessionLoop;
		public List<SceneEntitySnapshot> SceneEntities = new();
		public float BestDrilledDistance;
	}
}
