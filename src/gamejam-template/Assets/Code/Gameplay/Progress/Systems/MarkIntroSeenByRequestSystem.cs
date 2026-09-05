using System.Collections.Generic;
using Code.Infrastructure.EntityComponentSystem.Systems;
using Entitas;

namespace Code.Gameplay.Progress.Systems
{
	public sealed class MarkIntroSeenByRequestSystem : RequestHandlerSystem<GameEntity>
	{
		private readonly IGroup<GameEntity> _progresses;

		private readonly List<GameEntity> _buffer = new(1);

		public MarkIntroSeenByRequestSystem(GameContext game)
			: base(game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Request,
					GameMatcher.MarkIntroSeenRequest)))
		{
			_progresses = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamProgress)
				.NoneOf(
					GameMatcher.IntroSeen));
		}

		protected override void OnExecute(IGroup<GameEntity> requests)
		{
			foreach (GameEntity progress in _progresses.GetEntities(_buffer))
			{
				progress.isIntroSeen = true;
			}
		}
	}
}
