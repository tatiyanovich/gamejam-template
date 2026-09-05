using System.Collections.Generic;
using Code.Gameplay.Meow.Services;
using Entitas;

namespace Code.Gameplay.Meow.Systems
{
	public class RearmMeowOnQuietMicrophoneSystem : IExecuteSystem
	{
		private readonly IMeowConfigsService _meowConfigsService;

		private readonly IGroup<GameEntity> _disarmedMeowSources;

		private readonly List<GameEntity> _buffer = new(1);

		public RearmMeowOnQuietMicrophoneSystem(
			GameContext game,
			IMeowConfigsService meowConfigsService)
		{
			_meowConfigsService = meowConfigsService;

			_disarmedMeowSources = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.MeowSource,
					GameMatcher.MicrophoneLevel)
				.NoneOf(
					GameMatcher.MeowArmed,
					GameMatcher.OnCooldown));
		}

		public void Execute()
		{
			foreach (GameEntity meowSource in _disarmedMeowSources.GetEntities(_buffer))
			{
				if (meowSource.MicrophoneLevel >= _meowConfigsService.MeowConfig.RearmLevel)
					continue;

				meowSource.isMeowArmed = true;
			}
		}
	}
}
