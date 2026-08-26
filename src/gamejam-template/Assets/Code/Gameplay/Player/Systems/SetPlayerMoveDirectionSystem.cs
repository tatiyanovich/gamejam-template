using System.Collections.Generic;
using Code.Gameplay.Player.Services;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Player.Systems
{
	public sealed class SetPlayerMoveDirectionSystem : IExecuteSystem
	{
		private readonly IPlayerConfigsService _configsService;

		private readonly IGroup<InputEntity> _inputs;
		private readonly IGroup<GameEntity> _players;

		private readonly List<GameEntity> _buffer = new(4);

		public SetPlayerMoveDirectionSystem(GameContext game, InputContext input, IPlayerConfigsService configsService)
		{
			_configsService = configsService;

			_inputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.HorizontalAxis,
					InputMatcher.VerticalAxis));

			_players = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Player,
					GameMatcher.MovementDirection));
		}

		public void Execute()
		{
			float deadzone = _configsService.PlayerConfig.MoveDeadzone;

			foreach (InputEntity input in _inputs)
			{
				Vector3 axes = new(input.HorizontalAxis, input.VerticalAxis, 0f);

				foreach (GameEntity player in _players.GetEntities(_buffer))
				{
					if (axes.magnitude <= deadzone)
						player.ReplaceMovementDirection(Vector3.zero);
					else
						player.ReplaceMovementDirection(Vector3.ClampMagnitude(axes, 1f));
				}
			}
		}
	}
}
