using System;
using Code.Gameplay.Meow.Services;
using Code.Infrastructure.EntityComponentSystem;
using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Code.Infrastructure.Microphone;
using Entitas;

namespace Code.Gameplay.Meow.Queries
{
	public sealed class MeowQuery : IMeowQuery, IReactiveQuery
	{
		private readonly IMeowConfigsService _meowConfigsService;
		private readonly IMicrophoneService _microphoneService;

		private readonly IGroup<GameEntity> _meowSources;
		private readonly IGroup<GameEntity> _coolingMeowSources;
		private readonly IGroup<GameEntity> _changedMeowSources;
		private readonly IGroup<GameEntity> _meowEvents;

		public event Action<float> OnMicrophoneLevelChanged;
		public event Action OnMicrophoneTestPassed;

		public MeowQuery(
			GameContext game,
			IMeowConfigsService meowConfigsService,
			IMicrophoneService microphoneService)
		{
			_meowConfigsService = meowConfigsService;
			_microphoneService = microphoneService;

			_meowEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.MeowEvent));

			_meowSources = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.MeowSource,
					GameMatcher.MicrophoneLevel));

			_coolingMeowSources = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.MeowSource,
					GameMatcher.CooldownTimeLeft));

			_changedMeowSources = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.MeowSource,
					GameMatcher.MicrophoneLevel,
					GameMatcher.MicrophoneLevelChanged));
		}

		public void ReactToChanges()
		{
			foreach (GameEntity meowSource in _changedMeowSources)
				OnMicrophoneLevelChanged?.Invoke(meowSource.MicrophoneLevel);

			foreach (GameEntity meowEvent in _meowEvents)
			{
				if (meowEvent.meowEvent.FromMicrophone)
					OnMicrophoneTestPassed?.Invoke();
			}
		}

		public float GetMicrophoneLevel()
		{
			GameEntity meowSource = GetMeowSource();
			return meowSource == null ? 0f : meowSource.MicrophoneLevel;
		}

		public float GetThresholdLevel() => _meowConfigsService.MeowConfig.ThresholdLevel;

		public float GetRearmLevel() => _meowConfigsService.MeowConfig.RearmLevel;

		public float GetCooldownTimeLeft()
		{
			foreach (GameEntity meowSource in _coolingMeowSources)
				return meowSource.CooldownTimeLeft;

			return 0f;
		}

		public float GetCooldownSeconds() => _meowConfigsService.MeowConfig.CooldownSeconds;

		public bool IsArmed()
		{
			GameEntity meowSource = GetMeowSource();
			return meowSource != null && meowSource.isMeowArmed;
		}

		public bool IsOnCooldown()
		{
			GameEntity meowSource = GetMeowSource();
			return meowSource != null && meowSource.isOnCooldown;
		}

		public bool IsMicrophoneAvailable() => _microphoneService.IsAvailable;

		private GameEntity GetMeowSource()
		{
			foreach (GameEntity meowSource in _meowSources)
				return meowSource;

			return null;
		}
	}
}
