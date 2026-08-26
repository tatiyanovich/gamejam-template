using Code.Infrastructure.CoreLoop;

namespace Code.Gameplay.Camera.Services
{
	public interface ICameraSwitcher
	{
		void RegisterCamera(LoopNodeId node, UnityEngine.Camera camera);
		void UnregisterCamera(LoopNodeId node);
		void SwitchTo(LoopNodeId node);
	}
}
