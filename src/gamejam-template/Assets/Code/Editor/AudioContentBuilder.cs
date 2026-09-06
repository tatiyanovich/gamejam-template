using System;
using System.Collections.Generic;
using Code.Infrastructure.Audio.Configs;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Code.Editor
{
	public static class AudioContentBuilder
	{
		private const string AudioRoot = "Assets/AddressableResources/Content/Audio";
		private const string ConfigFolder = "Assets/AddressableResources/Configs/Audio";
		private const string ConfigPath = ConfigFolder + "/AudioConfig.asset";

		[MenuItem("COPYCAT/Audio/Build F1-F4")]
		public static void Build()
		{
			ConfigureImporters();
			AudioConfig config = LoadOrCreateConfig();
			ConfigureConfig(config);
			ConfigureAddressables(config);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Debug.Log("F1-F4 audio content and AudioConfig built successfully.");
		}

		private static void ConfigureImporters()
		{
			string[] assetGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { AudioRoot });
			foreach (string assetGuid in assetGuids)
			{
				string path = AssetDatabase.GUIDToAssetPath(assetGuid);
				AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
				bool streaming = path.Contains("/Music/") || path.EndsWith("heartbeat.wav");
				AudioImporterSampleSettings settings = importer.defaultSampleSettings;
				settings.compressionFormat = AudioCompressionFormat.Vorbis;
				settings.quality = streaming ? 0.72f : 0.82f;
				settings.loadType = streaming ? AudioClipLoadType.Streaming : AudioClipLoadType.DecompressOnLoad;
				settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
				settings.preloadAudioData = streaming == false;
				importer.defaultSampleSettings = settings;
				importer.forceToMono = path.Contains("/Music/") == false;
				importer.loadInBackground = streaming;
				importer.SaveAndReimport();
			}
		}

		private static AudioConfig LoadOrCreateConfig()
		{
			AudioConfig config = AssetDatabase.LoadAssetAtPath<AudioConfig>(ConfigPath);
			if (config != null)
				return config;

			EnsureFolder(ConfigFolder);
			config = ScriptableObject.CreateInstance<AudioConfig>();
			AssetDatabase.CreateAsset(config, ConfigPath);
			return config;
		}

		private static void ConfigureConfig(AudioConfig config)
		{
			SerializedObject serialized = new(config);
			SetArray(serialized, "meowFallback", new[]
			{
				Clip("Effects/meow_1.wav"),
				Clip("Effects/meow_2.wav"),
				Clip("Effects/meow_3.wav")
			});
			Set(serialized, "duckSqueak", Clip("Effects/duck_squeak.wav"));
			Set(serialized, "duckThrow", Clip("Effects/duck_throw.wav"));
			Set(serialized, "duckLand", Clip("Effects/duck_land.wav"));
			Set(serialized, "classLaugh", Clip("Effects/class_laugh.wav"));
			Set(serialized, "pencilSnap", Clip("Effects/pencil_snap.wav"));
			Set(serialized, "pencilScratch", Clip("Effects/pencil_scratch.wav"));
			Set(serialized, "answerCopied", Clip("Effects/answer_copied.wav"));
			Set(serialized, "chalkScratch", Clip("Effects/chalk_scratch.wav"));
			Set(serialized, "teacherHmm", Clip("Effects/teacher_hmm.wav"));
			Set(serialized, "heartbeat", Clip("Effects/heartbeat.wav"));
			Set(serialized, "clockTick", Clip("Effects/clock_tick.wav"));
			Set(serialized, "schoolBell", Clip("Effects/school_bell.wav"));
			Set(serialized, "teacherFootsteps", Clip("Effects/teacher_footsteps.wav"));
			Set(serialized, "timeWarning", Clip("Effects/time_warning.wav"));
			Set(serialized, "classroomMusic", Clip("Music/classroom.wav"));
			Set(serialized, "classroomUrgentMusic", Clip("Music/classroom_urgent.wav"));
			Set(serialized, "introPanelOne", Clip("VoiceOver/intro_panel_1.wav"));
			Set(serialized, "introPanelTwo", Clip("VoiceOver/intro_panel_2.wav"));
			Set(serialized, "introPanelThree", Clip("VoiceOver/intro_panel_3.wav"));
			Set(serialized, "introPanelFour", Clip("VoiceOver/intro_panel_4.wav"));
			serialized.FindProperty("microphoneDuckingMultiplier").floatValue = 0.25f;
			serialized.FindProperty("duckingTransitionSeconds").floatValue = 0.2f;
			serialized.FindProperty("heartbeatThreshold").floatValue = 0.8f;
			serialized.ApplyModifiedPropertiesWithoutUndo();
			EditorUtility.SetDirty(config);
		}

		private static void ConfigureAddressables(AudioConfig config)
		{
			AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
			AddressableAssetGroup audioGroup = settings.FindGroup("Audio");
			AddressableAssetGroup configGroup = settings.FindGroup("Configs");
			string[] assetGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { AudioRoot });

			foreach (string assetGuid in assetGuids)
			{
				string path = AssetDatabase.GUIDToAssetPath(assetGuid);
				AddressableAssetEntry entry = settings.CreateOrMoveEntry(assetGuid, audioGroup);
				entry.address = "audio_" + System.IO.Path.GetFileNameWithoutExtension(path);
			}

			string configGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(config));
			AddressableAssetEntry configEntry = settings.CreateOrMoveEntry(configGuid, configGroup);
			configEntry.address = "audio_config";
			settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, configEntry, true);
		}

		private static AudioClip Clip(string relativePath)
		{
			string path = AudioRoot + "/" + relativePath;
			AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
			if (clip == null)
				throw new InvalidOperationException($"Audio clip is missing at {path}");

			return clip;
		}

		private static void Set(SerializedObject serialized, string name, AudioClip clip)
		{
			serialized.FindProperty(name).objectReferenceValue = clip;
		}

		private static void SetArray(SerializedObject serialized, string name, IReadOnlyList<AudioClip> clips)
		{
			SerializedProperty property = serialized.FindProperty(name);
			property.arraySize = clips.Count;
			for (int index = 0; index < clips.Count; index++)
				property.GetArrayElementAtIndex(index).objectReferenceValue = clips[index];
		}

		private static void EnsureFolder(string path)
		{
			string[] segments = path.Split('/');
			string current = segments[0];
			for (int index = 1; index < segments.Length; index++)
			{
				string next = current + "/" + segments[index];
				if (AssetDatabase.IsValidFolder(next) == false)
					AssetDatabase.CreateFolder(current, segments[index]);

				current = next;
			}
		}
	}
}
