using System.Collections.Generic;
using Entitas;
using Framework.Essentials.TimeManagement;

namespace Code.Common.Cooldown.Systems
{
	public sealed class CooldownSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _cooldownEntities;
		
		private readonly ITimeService _timeService;

		private readonly List<GameEntity> _buffer = new(16);

		public CooldownSystem(
			GameContext gameContext,
			ITimeService timeService)
		{
			_timeService = timeService;
			_cooldownEntities = gameContext.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.OnCooldown,
					GameMatcher.CooldownTimeLeft));
		}

		public void Execute()
		{
			foreach (GameEntity cooldownEntity in _cooldownEntities.GetEntities(_buffer))
			{
				float timeLeft = cooldownEntity.CooldownTimeLeft - _timeService.DeltaTime;
				cooldownEntity.ReplaceCooldownTimeLeft(timeLeft);

				if (cooldownEntity.CooldownTimeLeft <= 0)
				{
					cooldownEntity.isOnCooldown = false;
				}
			}
		}
	}
}