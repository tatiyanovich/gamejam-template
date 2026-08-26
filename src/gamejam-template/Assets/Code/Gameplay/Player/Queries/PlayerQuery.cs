using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Player.Queries
{
	public class PlayerQuery : IPlayerQuery
	{
		private readonly IGroup<GameEntity> _players;
		private readonly List<GameEntity> _buffer = new(1);

		public PlayerQuery(GameContext game)
		{
			_players = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Player));
		}

		public bool TryGetPlayerPosition(out Vector3 position)
		{
			foreach (GameEntity player in _players.GetEntities(_buffer))
			{
				if (player.hasWorldPosition == false)
					break;

				position = player.WorldPosition;
				return true;
			}

			position = default;
			return false;
		}
	}
}
