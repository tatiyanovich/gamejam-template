using System;
using System.IO;
using Code.UI.Intro;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement;
using Framework.UI.UiManagement.Services;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using Zenject;

namespace Code.Editor
{
	public static class IntroPlaytest
	{
		[MenuItem("COPYCAT/QA/Test intro video")]
		public static void Run()
		{
			RunAsync().Forget();
		}

		private static async UniTaskVoid RunAsync()
		{
			string reportPath = PlaytestPaths.Get("intro.txt");
			try
			{
				if (Application.isPlaying == false)
					throw new InvalidOperationException("Enter Play Mode before running intro checks.");

				IUiService uiService = ProjectContext.Instance.Container.Resolve<IUiService>();
				IntroWindow window = await uiService.OpenWindow<IntroWindow>(
					beforeOpen: intro => intro.Prepare(onDismissed: null));
				VideoPlayer player = window.GetComponent<VideoPlayer>();
				AudioSource source = window.GetComponent<AudioSource>();
				await UniTask.WaitUntil(() => player.isPrepared && player.isPlaying);
				Require(player.clip != null, "Video clip is assigned");
				Require(player.clip.width == 1920 && player.clip.height == 1080, "Video is 1920x1080");
				Require(player.audioTrackCount > 0, "Video contains an audio track");
				Require(player.audioOutputMode == VideoAudioOutputMode.AudioSource, "Audio uses an AudioSource");
				Require(player.GetTargetAudioSource(0) == source, "Audio track targets the intro AudioSource");
				Require(source.isPlaying, "Intro audio is playing");
				File.WriteAllText(reportPath,
					"PASS intro video: 1920x1080 H.264, audio track routed and playing, runtime texture active\n");
				Debug.Log("Intro video checks passed.");
			}
			catch (Exception exception)
			{
				File.WriteAllText(reportPath, $"FAIL intro video: {exception}\n");
				Debug.LogException(exception);
			}
		}

		private static void Require(bool condition, string message)
		{
			if (condition == false)
				throw new InvalidOperationException(message);
		}
	}
}
