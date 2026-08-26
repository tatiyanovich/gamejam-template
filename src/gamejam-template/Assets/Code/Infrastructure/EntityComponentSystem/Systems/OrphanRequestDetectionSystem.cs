using Entitas;
using UnityEngine;

namespace Code.Infrastructure.EntityComponentSystem.Systems
{
	public class OrphanRequestDetectionSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _staleRequests;

		public OrphanRequestDetectionSystem(GameContext game)
		{
			_staleRequests = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Request,
					GameMatcher.StaleRequest));
		}

		public void Execute()
		{
			foreach (GameEntity request in _staleRequests)
			{
				Debug.LogError(
					$"[{nameof(OrphanRequestDetectionSystem)}] Request entity {request} survived a full frame without being destroyed. " +
					$"Ensure the handler extends {nameof(RequestHandlerSystem<GameEntity>)} or destroys the request manually.");
			}
		}
	}
}
