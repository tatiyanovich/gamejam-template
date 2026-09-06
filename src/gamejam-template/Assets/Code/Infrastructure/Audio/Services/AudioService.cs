using System;
using System.Collections.Generic;
using Code.Infrastructure.Microphone;
using Code.Infrastructure.Settings;
using Code.Infrastructure.Settings.Services;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Code.Infrastructure.Audio.Services
{
	public class AudioService : IAudioService, IInitializable, ITickable, IDisposable
	{
		private readonly IAudioConfigsService _audioConfigsService;
		private readonly IMicrophoneService _microphoneService;
		private readonly ISettingsService _settingsService;

		private readonly Dictionary<SfxId, AudioSource> _loopSources = new();

		private GameObject _root;
		private AudioSource _sfxSource;
		private AudioSource _musicSource;
		private AudioSource _voiceOverSource;
		private MusicId? _music;

		public AudioService(
			IAudioConfigsService audioConfigsService,
			IMicrophoneService microphoneService,
			ISettingsService settingsService)
		{
			_audioConfigsService = audioConfigsService;
			_microphoneService = microphoneService;
			_settingsService = settingsService;
		}

		public void Initialize()
		{
			_settingsService.OnSettingChanged += HandleSettingChanged;
		}

		public void Tick()
		{
			if (_musicSource == null || _musicSource.isPlaying == false)
				return;

			float multiplier = _microphoneService.IsAvailable
				? _audioConfigsService.AudioConfig.MicrophoneDuckingMultiplier
				: 1f;
			float transitionSeconds = _audioConfigsService.AudioConfig.DuckingTransitionSeconds;
			float step = transitionSeconds > 0f ? Time.unscaledDeltaTime / transitionSeconds : 1f;
			_musicSource.volume = Mathf.MoveTowards(_musicSource.volume, multiplier, step);
		}

		public void Dispose()
		{
			_settingsService.OnSettingChanged -= HandleSettingChanged;
			StopAllLoops();

			if (_root != null)
				Object.Destroy(_root);
		}

		public void PlaySfx(SfxId id)
		{
			if (_settingsService.IsEnabled(SettingTypeId.Effects) == false)
				return;

			EnsureSources();
			AudioClip clip = _audioConfigsService.AudioConfig.GetSfx(id);
			_sfxSource.PlayOneShot(clip);
		}

		public void StartLoop(SfxId id)
		{
			if (_settingsService.IsEnabled(SettingTypeId.Effects) == false)
				return;

			EnsureSources();

			if (_loopSources.TryGetValue(id, out AudioSource source) == false)
			{
				source = CreateSource(id.ToString());
				source.loop = true;
				source.clip = _audioConfigsService.AudioConfig.GetSfx(id);
				_loopSources.Add(id, source);
			}

			if (source.isPlaying == false)
				source.Play();
		}

		public void StopLoop(SfxId id)
		{
			if (_loopSources.TryGetValue(id, out AudioSource source))
				source.Stop();
		}

		public void StopAllLoops()
		{
			foreach (AudioSource source in _loopSources.Values)
				source.Stop();
		}

		public void PlayMusic(MusicId id)
		{
			EnsureSources();

			if (_music == id && _musicSource.isPlaying)
				return;

			_music = id;
			_musicSource.clip = _audioConfigsService.AudioConfig.GetMusic(id);
			_musicSource.volume = _microphoneService.IsAvailable
				? _audioConfigsService.AudioConfig.MicrophoneDuckingMultiplier
				: 1f;
			_musicSource.Play();
		}

		public void StopMusic()
		{
			if (_musicSource != null)
				_musicSource.Stop();

			_music = null;
		}

		public void PlayVoiceOver(VoiceOverId id)
		{
			if (_settingsService.IsEnabled(SettingTypeId.Effects) == false)
				return;

			EnsureSources();
			_voiceOverSource.clip = _audioConfigsService.AudioConfig.GetVoiceOver(id);
			_voiceOverSource.Play();
		}

		public void StopVoiceOver()
		{
			if (_voiceOverSource != null)
				_voiceOverSource.Stop();
		}

		public void RefreshSettings()
		{
			if (_root == null)
				return;

			bool effectsEnabled = _settingsService.IsEnabled(SettingTypeId.Effects);
			bool musicEnabled = _settingsService.IsEnabled(SettingTypeId.Music);
			_sfxSource.mute = effectsEnabled == false;
			_voiceOverSource.mute = effectsEnabled == false;
			_musicSource.mute = musicEnabled == false;

			foreach (AudioSource source in _loopSources.Values)
				source.mute = effectsEnabled == false;
		}

		private AudioSource CreateSource(string name)
		{
			GameObject node = new(name);
			node.transform.SetParent(_root.transform, false);
			AudioSource source = node.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.spatialBlend = 0f;
			return source;
		}

		private void EnsureSources()
		{
			if (_root != null)
				return;

			_root = new GameObject(nameof(AudioService));
			Object.DontDestroyOnLoad(_root);
			_sfxSource = CreateSource("Sfx");
			_musicSource = CreateSource("Music");
			_voiceOverSource = CreateSource("VoiceOver");
			_musicSource.loop = true;
			RefreshSettings();
		}

		private void HandleSettingChanged(SettingTypeId typeId)
		{
			if (typeId == SettingTypeId.Music || typeId == SettingTypeId.Effects)
				RefreshSettings();
		}
	}
}
