using System;
using System.IO;
using Code.Infrastructure.Audio;
using Code.Infrastructure.Audio.Configs;
using Code.Infrastructure.Audio.Services;
using Code.Infrastructure.Microphone;
using Code.Infrastructure.Settings;
using Code.Infrastructure.Settings.Services;
using Code.UI.Audio;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Zenject;
using FrameworkButton = Framework.UI.UiManagement.Elements.Buttons.Button;
using UnityButton = UnityEngine.UI.Button;

namespace Code.Editor
{
	public static class AudioPlaytest
	{
		[MenuItem("COPYCAT/QA/Test F1-F4 audio")]
		public static void Run()
		{
			RunAsync().Forget();
		}

		private static async UniTaskVoid RunAsync()
		{
			string reportPath = PlaytestPaths.Get("audio.txt");
			IAudioService audioService = null;
			ISettingsService settingsService = null;
			bool musicEnabled = true;
			bool effectsEnabled = true;
			try
			{
				if (Application.isPlaying == false)
					throw new InvalidOperationException("Enter Play Mode before running audio checks.");

				audioService = ProjectContext.Instance.Container.Resolve<IAudioService>();
				IAudioConfigsService configsService = ProjectContext.Instance.Container.Resolve<IAudioConfigsService>();
				IMicrophoneService microphoneService = ProjectContext.Instance.Container.Resolve<IMicrophoneService>();
				settingsService = ProjectContext.Instance.Container.Resolve<ISettingsService>();
				AudioConfig config = configsService.AudioConfig;
				ValidateConfig(config);
				musicEnabled = settingsService.IsEnabled(SettingTypeId.Music);
				effectsEnabled = settingsService.IsEnabled(SettingTypeId.Effects);
				settingsService.Toggle(SettingTypeId.Music, true);
				settingsService.Toggle(SettingTypeId.Effects, true);
				ValidateButtonAudio();

				GameObject root = GameObject.Find(nameof(AudioService));
				Require(root != null, "AudioService root was not created on the main menu");
				Require(Source(root, "Music").isPlaying, "Main-menu music did not start");
				Require(Source(root, "Music").clip == config.GetMusic(MusicId.MainMenu),
					"Main-menu music uses the wrong clip");
				ButtonClickAudio liveButton = UnityEngine.Object.FindFirstObjectByType<ButtonClickAudio>();
				Require(liveButton != null, "No live UI button has click audio");
				liveButton.SendMessage("HandleClicked");
				await UniTask.DelayFrame(1);
				Require(Source(root, "Sfx").isPlaying, "Live UI click SFX did not start");
				audioService.StopSfx();

				audioService.PlayMusic(MusicId.Classroom);
				audioService.PlaySfx(SfxId.ResultPassed);
				audioService.StartLoop(SfxId.Heartbeat);
				audioService.StartLoop(SfxId.ClockTick);
				audioService.PlayVoiceOver(VoiceOverId.IntroPanelTwo).Forget();
				await UniTask.DelayFrame(2);

				Require(Source(root, "Music").isPlaying, "Classroom music did not start");
				Require(Source(root, "Sfx").isPlaying, "Result-screen SFX did not start");
				Require(Source(root, "Heartbeat").isPlaying, "Heartbeat loop did not start");
				Require(Source(root, "ClockTick").isPlaying, "Final countdown tick did not start");
				Require(Source(root, "VoiceOver").isPlaying, "Intro voice-over did not start");
				float expectedMusicVolume = microphoneService.IsAvailable
					? config.MicrophoneDuckingMultiplier
					: 1f;
				Require(Mathf.Approximately(Source(root, "Music").volume, expectedMusicVolume),
					"Microphone ducking did not set the expected music volume");

				settingsService.Toggle(SettingTypeId.Music, false);
				settingsService.Toggle(SettingTypeId.Effects, false);
				Require(Source(root, "Music").mute, "Music setting did not mute music");
				Require(Source(root, "Sfx").mute, "Effects setting did not mute SFX");
				File.WriteAllText(reportPath,
					"PASS F1-F4 audio: clips, UI buttons, menu/result cues, loops, VO and settings\n");
				Debug.Log("F1-F4 audio checks passed.");
			}
			catch (Exception exception)
			{
				File.WriteAllText(reportPath, $"FAIL F1-F4 audio: {exception}\n");
				Debug.LogException(exception);
			}
			finally
			{
				if (settingsService != null)
				{
					settingsService.Toggle(SettingTypeId.Music, musicEnabled);
					settingsService.Toggle(SettingTypeId.Effects, effectsEnabled);
				}

				if (audioService != null)
				{
					audioService.StopMusic();
					audioService.StopAllLoops();
					audioService.StopVoiceOver();
				}
			}
		}

		private static void ValidateConfig(AudioConfig config)
		{
			foreach (SfxId id in Enum.GetValues(typeof(SfxId)))
			{
				AudioClip[] clips = config.GetSfxClips(id);
				Require(clips.Length > 0, $"No clips for {id}");
				foreach (AudioClip clip in clips)
					Require(clip != null && clip.length > 0f, $"Invalid clip for {id}");
			}

			foreach (MusicId id in Enum.GetValues(typeof(MusicId)))
				Require(config.GetMusic(id) != null, $"No music for {id}");

			foreach (VoiceOverId id in Enum.GetValues(typeof(VoiceOverId)))
				Require(config.GetVoiceOver(id) != null, $"No voice-over for {id}");
		}

		private static void ValidateButtonAudio()
		{
			string[] prefabGuids = AssetDatabase.FindAssets(
				"t:Prefab",
				new[] { "Assets/AddressableResources/Content/UI" });
			foreach (string prefabGuid in prefabGuids)
			{
				string path = AssetDatabase.GUIDToAssetPath(prefabGuid);
				GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
				foreach (UnityButton button in root.GetComponentsInChildren<UnityButton>(true))
					Require(button.GetComponent<ButtonClickAudio>() != null, $"No click SFX on {path}/{button.name}");

				foreach (FrameworkButton button in root.GetComponentsInChildren<FrameworkButton>(true))
					Require(button.GetComponent<ButtonClickAudio>() != null, $"No click SFX on {path}/{button.name}");
			}
		}

		private static AudioSource Source(GameObject root, string name)
		{
			Transform child = root.transform.Find(name);
			return child == null ? null : child.GetComponent<AudioSource>();
		}

		private static void Require(bool condition, string message)
		{
			if (condition == false)
				throw new InvalidOperationException(message);
		}
	}
}
