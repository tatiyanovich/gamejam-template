using System.Collections.Generic;
using Code.Gameplay.Debugging.Services;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Debugging.Systems
{
	public sealed class TriggerGameplayDebugActionsSystem : IExecuteSystem
	{
		private readonly List<IGameplayDebugInputAction> _actions;
		private readonly IGroup<InputEntity> _inputs;

		public TriggerGameplayDebugActionsSystem(List<IGameplayDebugInputAction> actions, InputContext input)
		{
			_actions = actions;
			_inputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.PointerWorldPosition));
		}

		public void Execute()
		{
			Vector3 pointerWorldPosition = GetPointerWorldPosition();

			foreach (IGameplayDebugInputAction action in _actions)
			{
				if (action.WasTriggeredThisFrame() == false)
					continue;

				action.Execute(pointerWorldPosition);
			}
		}

		private Vector3 GetPointerWorldPosition()
		{
			foreach (InputEntity input in _inputs)
				return input.PointerWorldPosition;

			return Vector3.zero;
		}
	}
}
