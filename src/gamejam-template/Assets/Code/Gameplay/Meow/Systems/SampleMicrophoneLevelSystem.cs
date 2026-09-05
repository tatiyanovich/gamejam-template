using System.Collections.Generic;
using Code.Gameplay.Meow.Services;
using Code.Infrastructure.Microphone;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Meow.Systems
{
	public class SampleMicrophoneLevelSystem : IExecuteSystem
	{
		private readonly IMicrophoneService _microphoneService;
		private readonly IMeowConfigsService _meowConfigsService;

		private const float MaximumLevel = 100f;

		private readonly IGroup<GameEntity> _meowSources;

		private readonly List<GameEntity> _buffer = new(1);

		public SampleMicrophoneLevelSystem(
			GameContext game,
			IMicrophoneService microphoneService,
			IMeowConfigsService meowConfigsService)
		{
			_microphoneService = microphoneService;
			_meowConfigsService = meowConfigsService;

			_meowSources = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.MeowSource,
					GameMatcher.MicrophoneLevel));
		}

		public void Execute()
		{
			float rootMeanSquare = _microphoneService.GetRootMeanSquare();
			float level = Mathf.Min(MaximumLevel, rootMeanSquare * _meowConfigsService.MeowConfig.LevelScale);

			foreach (GameEntity meowSource in _meowSources.GetEntities(_buffer))
			{
				meowSource.ReplaceMicrophoneLevel(level);
			}
		}
	}
}
