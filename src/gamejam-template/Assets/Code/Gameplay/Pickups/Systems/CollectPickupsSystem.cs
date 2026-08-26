using System.Collections.Generic;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Pickups.Systems
{
	public class CollectPickupsSystem : IExecuteSystem
	{
		private readonly IEntityFactory _entityFactory;

		private readonly IGroup<GameEntity> _players;
		private readonly IGroup<GameEntity> _pickups;

		private readonly List<GameEntity> _buffer = new(32);

		public CollectPickupsSystem(GameContext game, IEntityFactory entityFactory)
		{
			_entityFactory = entityFactory;

			_players = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Player,
					GameMatcher.WorldPosition));

			_pickups = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Pickup,
					GameMatcher.ScoreValue,
					GameMatcher.CollectRadius,
					GameMatcher.WorldPosition)
				.NoneOf(
					GameMatcher.Destructed));
		}

		public void Execute()
		{
			foreach (GameEntity player in _players)
			foreach (GameEntity pickup in _pickups.GetEntities(_buffer))
			{
				if (Vector3.Distance(player.WorldPosition, pickup.WorldPosition) > pickup.CollectRadius)
					continue;

				pickup.isDestructed = true;

				_entityFactory.Event()
					.AddPickupCollectedEvent(pickup.ScoreValue);
			}
		}
	}
}
