using System.Collections.Generic;
using Entitas;

namespace Code.Infrastructure.EntityComponentSystem.SceneEntities.Systems
{
	public class InitializeSceneEntitiesByRequestSystem : IInitializeSystem, IExecuteSystem
	{
		private readonly IGroup<GameEntity> _requests;
		private readonly List<GameEntity> _buffer = new(128);

		public InitializeSceneEntitiesByRequestSystem(GameContext game)
		{
			_requests = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.InitializeSceneEntityRequest));
		}

		public void Initialize()
		{
			ProcessRequests();
		}

		public void Execute()
		{
			ProcessRequests();
		}

		private void ProcessRequests()
		{
			foreach (GameEntity request in _requests.GetEntities(_buffer))
			{
				request.InitializeSceneEntityRequest.Initialize();
			}

			foreach (GameEntity request in _requests.GetEntities(_buffer))
			{
				request.InitializeSceneEntityRequest.ResolveReferences();
			}

			foreach (GameEntity request in _requests.GetEntities(_buffer))
			{
				request.Destroy();
			}
		}
	}
}