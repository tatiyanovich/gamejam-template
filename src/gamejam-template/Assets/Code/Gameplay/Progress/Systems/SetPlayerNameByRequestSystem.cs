using System.Collections.Generic;
using Code.Infrastructure.EntityComponentSystem.Systems;
using Entitas;

namespace Code.Gameplay.Progress.Systems
{
	public sealed class SetPlayerNameByRequestSystem : RequestHandlerSystem<GameEntity>
	{
		private readonly IGroup<GameEntity> _progresses;

		private readonly List<GameEntity> _requestBuffer = new(1);
		private readonly List<GameEntity> _progressBuffer = new(1);

		public SetPlayerNameByRequestSystem(GameContext game)
			: base(game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Request,
					GameMatcher.SetPlayerNameRequest)))
		{
			_progresses = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamProgress,
					GameMatcher.PlayerName));
		}

		protected override void OnExecute(IGroup<GameEntity> requests)
		{
			foreach (GameEntity request in requests.GetEntities(_requestBuffer))
			{
				foreach (GameEntity progress in _progresses.GetEntities(_progressBuffer))
				{
					progress.ReplacePlayerName(request.SetPlayerNameRequest);
				}
			}
		}
	}
}
