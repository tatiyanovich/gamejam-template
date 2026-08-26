using Code.Infrastructure.CoreLoop;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Code.Infrastructure.Scenes
{
	// Keeps the SceneInstance returned by LoadScene so it can later be unloaded by node.
	// Unloading needs the original instance — a default SceneInstance has no valid handle.
	public interface ILoadedSceneRegistry
	{
		void Register(LoopNodeId nodeId, SceneInstance scene);
		bool TryGet(LoopNodeId nodeId, out SceneInstance scene);
		void Remove(LoopNodeId nodeId);
	}
}
