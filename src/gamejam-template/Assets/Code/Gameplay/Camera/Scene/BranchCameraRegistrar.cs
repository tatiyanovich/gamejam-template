using Code.Gameplay.Camera.Services;
using Code.Infrastructure.CoreLoop;
using UnityEngine;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Camera.Scene
{
	[RequireComponent(typeof(UnityEngine.Camera))]
	public class BranchCameraRegistrar : MonoBehaviour
	{
		[SF] private LoopNodeId node;

		private UnityEngine.Camera _camera;
		private ICameraSwitcher _cameraSwitch;

		[Inject]
		private void Construct(ICameraSwitcher cameraSwitch)
		{
			_cameraSwitch = cameraSwitch;
		}

		private void Awake() => _camera = GetComponent<UnityEngine.Camera>();

		private void Start() => _cameraSwitch.RegisterCamera(node, _camera);

		private void OnDestroy() => _cameraSwitch.UnregisterCamera(node);
	}
}
