using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Vfx.Systems
{
	public sealed class DestructCompletedVfxSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _particles;
		private readonly List<GameEntity> _buffer = new(16);

		public DestructCompletedVfxSystem(GameContext gameContext)
		{
			_particles = gameContext.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Vfx, 
					GameMatcher.VfxAnimator)
				.NoneOf(
					GameMatcher.Destructed));
		}

		public void Execute()
		{
			foreach (GameEntity vfx in _particles.GetEntities(_buffer))
			{
				if (vfx.VfxAnimator.IsPlaying())
					continue;

				vfx.isDestructed = true;
			}
		}
	}
}
