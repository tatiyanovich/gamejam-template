using System;
using System.IO;
using System.Text;
using Code.Gameplay.Camera.Services;
using Code.Infrastructure.Microphone;
using Entitas;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Code.Editor
{
	[InitializeOnLoad]
	public static class GameplayPlaytest
	{
		private static string _lastCommand;
		private static string _lastCaptureState;
		private static string _recordingDirectory;
		private static double _nextSample;
		private static bool _recording;
		private static bool _useGreybox;
		private static GreyboxPlaytestView _greybox;
		private static MicrophoneService _mutedMicrophone;

		static GameplayPlaytest()
		{
			EditorApplication.update += Update;
			EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
			IgnorePreviousCommand();
		}

		[MenuItem("COPYCAT/QA/Record gameplay")]
		public static void Record()
		{
			if (EditorApplication.isPlaying == false)
				throw new InvalidOperationException("Enter Play Mode before recording gameplay.");

			_recordingDirectory = PlaytestPaths.Get(DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff"));
			Directory.CreateDirectory(_recordingDirectory);
			_lastCaptureState = null;
			_recording = true;
			FocusGameView();
		}

		[MenuItem("COPYCAT/QA/Stop recording")]
		public static void StopRecording() => _recording = false;

		[MenuItem("COPYCAT/QA/Use keyboard meow")]
		public static void UseKeyboardMeow()
		{
			SceneContext scene = GetGameplayScene();
			if (scene == null || _mutedMicrophone != null)
				return;
			_mutedMicrophone = scene.Container.Resolve<IMicrophoneService>() as MicrophoneService;
			_mutedMicrophone?.Dispose();
		}

		[MenuItem("COPYCAT/QA/Use microphone")]
		public static void UseMicrophone()
		{
			_mutedMicrophone?.Initialize();
			_mutedMicrophone = null;
		}

		[MenuItem("COPYCAT/QA/Use diagnostic view")]
		public static void UseGreybox()
		{
			_useGreybox = true;
			FocusGameView();
		}

		[MenuItem("COPYCAT/QA/Use game art")]
		public static void UseGameArt()
		{
			_useGreybox = false;
			_greybox?.Dispose();
			_greybox = null;
		}

		[MenuItem("COPYCAT/QA/Capture Game View")]
		public static void CaptureGameView()
		{
			ScreenCapture.CaptureScreenshot(PlaytestPaths.Get($"game-view-{Time.frameCount}.png"));
		}

		private static void Update()
		{
			if (EditorApplication.isPlaying == false || EditorApplication.isCompiling)
				return;

			try
			{
				ProcessCommand();
				SceneContext scene = GetGameplayScene();
				UpdateGreybox(scene);
				if (_recording == false || EditorApplication.timeSinceStartup < _nextSample)
					return;

				_nextSample = EditorApplication.timeSinceStartup + 0.2;
				WriteSnapshot(scene);
			}
			catch (Exception exception)
			{
				_recording = false;
				_useGreybox = false;
				File.WriteAllText(PlaytestPaths.Get("error.txt"), exception.ToString());
				Debug.LogException(exception);
			}
		}

		private static void ProcessCommand()
		{
			string path = PlaytestPaths.Get("command.txt");
			if (File.Exists(path) == false)
				return;

			string command = File.ReadAllText(path).Trim();
			if (command == _lastCommand)
				return;

			_lastCommand = command;
			switch (command.Split(' ')[0])
			{
				case "record": Record(); break;
				case "end": StopRecording(); break;
				case "greybox": UseGreybox(); break;
				case "art": UseGameArt(); break;
				case "keyboard": UseKeyboardMeow(); break;
				case "microphone": UseMicrophone(); break;
				case "focus": FocusGameView(); break;
				case "snapshot": WriteSnapshot(GetGameplayScene()); break;
				case "capture": CaptureCamera(GetGameplayScene(), PlaytestPaths.Get("camera.png")); break;
				case "gameview": CaptureGameView(); break;
				default: throw new InvalidOperationException($"Unknown playtest command: {command}");
			}
		}

		private static void UpdateGreybox(SceneContext scene)
		{
			if (_greybox != null && _greybox.Scene != scene)
			{
				_greybox.Dispose();
				_greybox = null;
			}
			if (_useGreybox == false || scene == null)
				return;

			_greybox ??= new GreyboxPlaytestView(scene);
			_greybox.Update();
		}

		private static SceneContext GetGameplayScene()
		{
			foreach (SceneContext scene in UnityEngine.Object.FindObjectsByType<SceneContext>(FindObjectsSortMode.None))
			{
				if (scene.gameObject.scene.name == "Gameplay")
					return scene;
			}
			return null;
		}

		private static void WriteSnapshot(SceneContext scene)
		{
			StringBuilder state = new();
			state.AppendLine($"Frame={Time.frameCount} Time={Time.time} Scale={Time.timeScale}");
			state.AppendLine($"KeyboardMeowOnly={_mutedMicrophone != null}");
			if (scene == null)
			{
				File.WriteAllText(PlaytestPaths.Get("state.txt"), state.AppendLine("No gameplay scene").ToString());
				return;
			}

			GameContext game = scene.Container.Resolve<GameContext>();
			InputContext input = scene.Container.Resolve<InputContext>();
			foreach (GameEntity entity in game.GetEntities())
			{
				if (entity.isExamRun || entity.isQuestion || entity.isTeacher || entity.isNeighbour
					|| entity.isDuck || entity.isExamProgress || entity.isMeowSource)
					AppendEntity(state, entity);
			}
			foreach (InputEntity entity in input.GetEntities())
				AppendEntity(state, entity);
			File.WriteAllText(PlaytestPaths.Get("state.txt"), state.ToString());
			if (_recording == false)
				return;

			File.AppendAllText(Path.Combine(_recordingDirectory, "states.txt"), state.ToString());
			string captureState = GetCaptureState(game);
			if (captureState == _lastCaptureState)
				return;
			_lastCaptureState = captureState;
			string imagePath = Path.Combine(_recordingDirectory, $"frame-{Time.frameCount}.png");
			CaptureCamera(scene, imagePath);
			File.WriteAllText(Path.ChangeExtension(imagePath, ".txt"), state.ToString());
		}

		private static void AppendEntity(StringBuilder state, IEntity entity)
		{
			foreach (IComponent component in entity.GetComponents())
				state.Append($"{component.GetType().Name}={JsonUtility.ToJson(component)} ");
			state.AppendLine();
		}

		private static string GetCaptureState(GameContext game)
		{
			StringBuilder state = new();
			foreach (GameEntity run in game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.Id,
					GameMatcher.ExamOutcome)))
				state.Append($"{run.Id}:{run.ExamOutcome};");
			foreach (GameEntity question in game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question,
					GameMatcher.QuestionIndex,
					GameMatcher.AnswerProgress)))
				state.Append($"{question.QuestionIndex}:{question.AnswerProgress}:{question.isAnswerReadable};");
			foreach (GameEntity teacher in game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherAttention)))
				state.Append($"{teacher.TeacherAttention};");
			foreach (GameEntity duck in game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Duck,
					GameMatcher.DuckState)))
				state.Append($"{duck.DuckState};");
			return state.ToString();
		}

		private static void CaptureCamera(SceneContext scene, string path)
		{
			if (scene == null)
				return;
			UnityEngine.Camera camera = scene.Container.Resolve<ICameraQuery>().GetCamera();
			if (camera == null)
				return;

			RenderTexture texture = new(1920, 1080, 24);
			RenderTexture previous = RenderTexture.active;
			Texture2D image = new(1920, 1080, TextureFormat.RGB24, false);
			try
			{
				UniversalRenderPipeline.SingleCameraRequest request = new() { destination = texture };
				RenderPipeline.SubmitRenderRequest(camera, request);
				RenderTexture.active = texture;
				image.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
				image.Apply();
				File.WriteAllBytes(path, image.EncodeToPNG());
			}
			finally
			{
				RenderTexture.active = previous;
				UnityEngine.Object.DestroyImmediate(image);
				UnityEngine.Object.DestroyImmediate(texture);
			}
		}

		private static void FocusGameView()
		{
			foreach (EditorWindow window in Resources.FindObjectsOfTypeAll<EditorWindow>())
			{
				if (window.GetType().Name == "GameView")
					window.Focus();
			}
		}

		private static void IgnorePreviousCommand()
		{
			string path = PlaytestPaths.Get("command.txt");
			_lastCommand = File.Exists(path) ? File.ReadAllText(path).Trim() : null;
		}

		private static void HandlePlayModeStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.EnteredPlayMode)
				IgnorePreviousCommand();
			if (state != PlayModeStateChange.ExitingPlayMode)
				return;
			StopRecording();
			UseGameArt();
			_mutedMicrophone = null;
		}
	}
}
