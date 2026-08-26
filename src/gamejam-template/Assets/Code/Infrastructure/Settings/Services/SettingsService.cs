using System;
using Code.Storage.SaveFiles;
using Framework.Storage;
using UnityEngine;

namespace Code.Infrastructure.Settings.Services
{
	public class SettingsService : ISettingsService
	{
		private readonly ISaveLoadService _saveLoadService;

		public event Action<SettingTypeId> OnSettingChanged;

		public FramerateTypeId Framerate { get; private set; }

		public SettingsService(ISaveLoadService saveLoadService)
		{
			_saveLoadService = saveLoadService;
		}

		public void LoadProgress()
		{
			SettingsSaveFile saveFile = _saveLoadService.Get<SettingsSaveFile>();

			Apply(SettingTypeId.Music, saveFile.MusicEnabled);
			Apply(SettingTypeId.Effects, saveFile.EffectsEnabled);
			Apply(SettingTypeId.Quality, saveFile.HighQuality);

			SetTargetFramerate(GetFramerateType(saveFile.Framerate));
		}

		public void Toggle(SettingTypeId typeId, bool value)
		{
			if (IsEnabled(typeId) == value)
				return;

			SettingsSaveFile saveFile = _saveLoadService.Get<SettingsSaveFile>();

			switch (typeId)
			{
				case SettingTypeId.Music:
					saveFile.MusicEnabled = value;
					break;
				case SettingTypeId.Effects:
					saveFile.EffectsEnabled = value;
					break;
				case SettingTypeId.Quality:
					saveFile.HighQuality = value;
					break;
				default:
					Debug.LogError($"Trying to toggle unknown setting {typeId}");
					return;
			}

			Apply(typeId, value);
			OnSettingChanged?.Invoke(typeId);
		}

		public void SetTargetFramerate(FramerateTypeId framerateTypeId)
		{
			if (framerateTypeId == FramerateTypeId.Unknown)
			{
				framerateTypeId = FramerateTypeId.Mid;
			}

			int targetFramerate = GetFramerateValue(framerateTypeId);

			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = targetFramerate;
			Framerate = framerateTypeId;

			_saveLoadService.Get<SettingsSaveFile>().Framerate = targetFramerate;
			
			OnSettingChanged?.Invoke(SettingTypeId.Framerate);
		}

		public bool IsEnabled(SettingTypeId typeId)
		{
			SettingsSaveFile saveFile = _saveLoadService.Get<SettingsSaveFile>();

			return typeId switch
			{
				SettingTypeId.Music => saveFile.MusicEnabled,
				SettingTypeId.Effects => saveFile.EffectsEnabled,
				SettingTypeId.Quality => saveFile.HighQuality,
				_ => false
			};
		}

		public int GetFramerateValue(FramerateTypeId framerateTypeId)
		{
			return framerateTypeId switch
			{
				FramerateTypeId.Low => Constants.GraphicsQuality.LowFramerate,
				FramerateTypeId.Mid => Constants.GraphicsQuality.MidFramerate,
				FramerateTypeId.Max => Constants.GraphicsQuality.MaxFramerate,
				_ => throw new ArgumentOutOfRangeException(nameof(framerateTypeId), framerateTypeId, null)
			};
		}

		private FramerateTypeId GetFramerateType(int framerate)
		{
			return framerate switch
			{
				Constants.GraphicsQuality.LowFramerate => FramerateTypeId.Low,
				Constants.GraphicsQuality.MidFramerate => FramerateTypeId.Mid,
				Constants.GraphicsQuality.MaxFramerate => FramerateTypeId.Max,
				_ => FramerateTypeId.Unknown
			};
		}

		private void Apply(SettingTypeId typeId, bool value)
		{
			switch (typeId)
			{
				case SettingTypeId.Music:
				case SettingTypeId.Effects:
					ApplyMasterVolume();
					break;
				case SettingTypeId.Quality:
					SetQuality(value);
					break;
				default:
					Debug.LogError($"Trying to apply setting for type {typeId}");
					break;
			}
		}

		private void ApplyMasterVolume()
		{
			SettingsSaveFile saveFile = _saveLoadService.Get<SettingsSaveFile>();
			bool soundEnabled = saveFile.MusicEnabled || saveFile.EffectsEnabled;

			AudioListener.volume = soundEnabled ? 1f : 0f;
		}

		private void SetQuality(bool highQuality)
		{
			int qualityIndex = highQuality ? 1 : 0;
			QualitySettings.SetQualityLevel(qualityIndex, true);
		}
	}
}
