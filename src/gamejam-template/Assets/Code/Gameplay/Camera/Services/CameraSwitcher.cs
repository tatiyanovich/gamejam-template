using System.Collections.Generic;
using Code.Infrastructure.CoreLoop;

namespace Code.Gameplay.Camera.Services
{
	public class CameraSwitcher : ICameraSwitcher
	{
		private readonly Dictionary<LoopNodeId, UnityEngine.Camera> _cameras = new(4);

		private LoopNodeId _activeNode = LoopNodeId.Unknown;

		public void RegisterCamera(LoopNodeId node, UnityEngine.Camera camera)
		{
			if (camera == null)
				return;

			_cameras[node] = camera;
			camera.enabled = node == _activeNode;
		}

		public void UnregisterCamera(LoopNodeId node)
		{
			_cameras.Remove(node);
		}

		public void SwitchTo(LoopNodeId node)
		{
			_activeNode = node;

			foreach (KeyValuePair<LoopNodeId, UnityEngine.Camera> pair in _cameras)
			{
				if (pair.Value == null)
					continue;

				pair.Value.enabled = pair.Key == node;
			}
		}
	}
}
