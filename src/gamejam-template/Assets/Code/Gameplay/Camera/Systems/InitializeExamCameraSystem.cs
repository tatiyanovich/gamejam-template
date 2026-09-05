using Code.Gameplay.Camera.Services;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Camera.Systems
{
	public class InitializeExamCameraSystem : IInitializeSystem
	{
		private readonly ICameraFactory _cameraFactory;

		public InitializeExamCameraSystem(ICameraFactory cameraFactory)
		{
			_cameraFactory = cameraFactory;
		}

		public void Initialize()
		{
			_cameraFactory.CreateStaticCamera(Vector3.zero);
		}
	}
}
