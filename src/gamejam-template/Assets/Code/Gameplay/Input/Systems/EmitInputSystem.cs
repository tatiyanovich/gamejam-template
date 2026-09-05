using Code.Gameplay.Camera.Services;
using Code.Gameplay.Exam;
using Code.Gameplay.Input.Data;
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
			foreach (InputEntity input in _inputs)
			{
				input.isLeanHeld = _inputService.IsKeyHeld(InputKeyMap.Lean);
				input.isMeowKeyPressed = _inputService.IsKeyPressed(InputKeyMap.Meow);
				input.isDuckKeyPressed = _inputService.IsKeyPressed(InputKeyMap.Duck);

				EmitStroke(input);
				EmitPick(input);
				EmitLetter(input);
				EmitPointerWorldPosition(input);
			}
		}

		private void EmitStroke(InputEntity input)
		{
			foreach (KeyBinding<StrokeDirection> binding in InputKeyMap.Strokes)
			{
				if (_inputService.IsKeyPressed(binding.Key) == false)
					continue;

				input.ReplaceStrokeInput(binding.Value);
				return;
			}

			input.SafeRemoveStrokeInput();
		}

		private void EmitPick(InputEntity input)
		{
			foreach (KeyBinding<int> binding in InputKeyMap.Picks)
			{
				if (_inputService.IsKeyPressed(binding.Key) == false)
					continue;

				input.ReplacePickInput(binding.Value);
				return;
			}

			input.SafeRemovePickInput();
		}

		private void EmitLetter(InputEntity input)
		{
			foreach (KeyBinding<char> binding in InputKeyMap.Letters)
			{
				if (_inputService.IsKeyPressed(binding.Key) == false)
					continue;

				input.ReplaceLetterInput(binding.Value);
				return;
			}

			input.SafeRemoveLetterInput();
		}

		private void EmitPointerWorldPosition(InputEntity input)
		{
			UnityEngine.Camera camera = _cameraQuery.GetCamera();

			if (camera == null)
				return;

			Vector3 pointerWorldPosition = camera.ScreenToWorldPoint(_inputService.GetPointerScreenPosition());
			pointerWorldPosition.z = 0f;

			input.ReplacePointerWorldPosition(pointerWorldPosition);
		}
	}
}
