using System.Collections.Generic;
using Code.Infrastructure.CoreLoop;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Code.Infrastructure.Scenes
{
	public class LoadedSceneRegistry : ILoadedSceneRegistry
	{
		private readonly Dictionary<LoopNodeId, SceneInstance> _scenes = new(4);

		public void Register(LoopNodeId nodeId, SceneInstance scene) => _scenes[nodeId] = scene;

		public bool TryGet(LoopNodeId nodeId, out SceneInstance scene) => _scenes.TryGetValue(nodeId, out scene);

		public void Remove(LoopNodeId nodeId) => _scenes.Remove(nodeId);
	}
}
