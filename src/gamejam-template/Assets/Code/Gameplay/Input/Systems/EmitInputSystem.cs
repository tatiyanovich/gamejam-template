using Code.Gameplay.Camera.Services;
using Code.Infrastructure.Input;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Input.Systems
{
	public class EmitInputSystem : IExecuteSystem
	{
		private readonly IInputService _inputService;
		private readonly ICameraQuery _cameraQuery;
		private readonly IGroup<InputEntity> _inputs;

		public EmitInputSystem(
			IInputService inputService,
			ICameraQuery cameraQuery,
			InputContext input)
		{
			_inputService = inputService;
			_cameraQuery = cameraQuery;

			_inputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input));
		}

		public void Execute()
		{
			UnityEngine.Camera camera = _cameraQuery.GetCamera();

			foreach (InputEntity input in _inputs)
			{
				input.ReplaceHorizontalAxis(_inputService.GetHorizontalAxis());
				input.ReplaceVerticalAxis(_inputService.GetVerticalAxis());

				if (camera != null)
				{
					Vector3 pointerWorld = camera.ScreenToWorldPoint(_inputService.GetPointerScreenPosition());
					pointerWorld.z = 0f;
					input.ReplacePointerWorldPosition(pointerWorld);
				}
			}
		}
	}
}
