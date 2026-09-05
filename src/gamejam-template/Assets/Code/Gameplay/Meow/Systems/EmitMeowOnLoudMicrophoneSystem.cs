using System.Collections.Generic;
using Code.Common.Cooldown;
using Code.Gameplay.Meow.Services;
using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;

namespace Code.Gameplay.Meow.Systems
{
	public class EmitMeowOnLoudMicrophoneSystem : IExecuteSystem
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IMeowConfigsService _meowConfigsService;

		private readonly IGroup<GameEntity> _armedMeowSources;

		private readonly List<GameEntity> _buffer = new(1);

		public EmitMeowOnLoudMicrophoneSystem(
			GameContext game,
			IEntityFactory entityFactory,
			IMeowConfigsService meowConfigsService)
		{
			_entityFactory = entityFactory;
			_meowConfigsService = meowConfigsService;

			_armedMeowSources = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.MeowSource,
					GameMatcher.MeowArmed,
					GameMatcher.MicrophoneLevel)
				.NoneOf(
					GameMatcher.OnCooldown));
		}

		public void Execute()
		{
			foreach (GameEntity meowSource in _armedMeowSources.GetEntities(_buffer))
			{
				if (meowSource.MicrophoneLevel < _meowConfigsService.MeowConfig.ThresholdLevel)
					continue;

				meowSource.isMeowArmed = false;
				meowSource.PutOnCooldown(_meowConfigsService.MeowConfig.CooldownSeconds);

				_entityFactory.Event()
					.AddMeowEvent(true);
			}
		}
	}
}
