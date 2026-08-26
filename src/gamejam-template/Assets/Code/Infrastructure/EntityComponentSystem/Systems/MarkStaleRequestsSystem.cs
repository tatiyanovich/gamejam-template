using System.Collections.Generic;
using Entitas;

namespace Code.Infrastructure.EntityComponentSystem.Systems
{
	public class MarkStaleRequestsSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _requests;
		private readonly List<GameEntity> _buffer = new(64);

		public MarkStaleRequestsSystem(GameContext game)
		{
			_requests = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Request)
				.NoneOf(
					GameMatcher.StaleRequest));
		}

		public void Execute()
		{
			foreach (GameEntity request in _requests.GetEntities(_buffer))
			{
				request.isStaleRequest = true;
			}
		}
	}
}
