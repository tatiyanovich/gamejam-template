using System;
using System.IO;
using System.Text;
using Code.Gameplay.Bell.Queries;
using Code.Gameplay.Difficulty.Services;
using Code.Gameplay.Duck;
using Code.Gameplay.Duck.Queries;
using Code.Gameplay.Duck.Services;
using Code.Gameplay.Exam;
using Code.Gameplay.Exam.Queries;
using Code.Gameplay.Meow.Queries;
using Code.Gameplay.Meow.Services;
using Code.Gameplay.Meow.Systems;
using Code.Gameplay.Neighbours;
using Code.Gameplay.Neighbours.Queries;
using Code.Gameplay.Neighbours.Services;
using Code.Gameplay.Suspicion.Queries;
using Code.Gameplay.Teacher;
using Code.Gameplay.Teacher.Queries;
using Code.Gameplay.Teacher.Services;
using Code.Infrastructure.EntityComponentSystem.Events.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.UI.Gameplay;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Zenject;
using Object = UnityEngine.Object;

namespace Code.Editor
{
	public static class GameplayHudPlaytest
	{
		[MenuItem("COPYCAT/QA/Test gameplay HUD")]
		public static void Run() => RunChecks().Forget();

		[MenuItem("COPYCAT/QA/Preview HUD risk")]
		public static void PreviewRisk() => ShowRisk().Forget();

		private static async UniTask ShowRisk()
		{
			GameplayWindow window = Object.FindFirstObjectByType<GameplayWindow>();
			if (EditorApplication.isPlaying == false || window == null)
				throw new InvalidOperationException("Enter the exam before previewing HUD risk.");

			Time.timeScale = 0f;
			GameContext game = ProjectContext.Instance.Container.Resolve<GameContext>();
			foreach (GameEntity run in game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun)))
			{
				run.isExamFinished = false;
				run.ReplaceAnswersCopied(7);
				run.ReplaceExamElapsedSeconds(76f);
				run.ReplaceSuspicionLevel(85f);
				run.isBellAnnounced = true;
			}
			foreach (GameEntity teacher in game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher)))
				teacher.SwitchAttention(TeacherAttention.Staring, 10f);
			foreach (GameEntity neighbour in game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Neighbour)))
			{
				neighbour.isPawLifted = true;
				neighbour.ReplacePawWindowTimeLeft(1f);
			}
			await ((IWindowControl)window).Close(false, default);
			await ((IWindowControl)window).Open(false, default);
		}

		private static async UniTask RunChecks()
		{
			StringBuilder report = new();
			GameplayWindow live = Object.FindFirstObjectByType<GameplayWindow>();
			GameObject instance = null;
			GameObject ring = null;
			IWindowControl control = null;
			try
			{
				if (EditorApplication.isPlaying == false || live == null)
					throw new InvalidOperationException("Enter the exam before running HUD checks.");
				SceneContext scene = null;
				foreach (SceneContext candidate in Object.FindObjectsByType<SceneContext>(FindObjectsSortMode.None))
				{
					if (candidate.gameObject.scene.name == "Gameplay")
						scene = candidate;
				}
				Canvas.ForceUpdateCanvases();
				Button clickable = Field<Button>(new SerializedObject(live), "duckButton");
				RectTransform clickBounds = (RectTransform)clickable.transform;
				Vector3 clickWorld = clickBounds.TransformPoint(clickBounds.rect.center);
				Vector2 clickScreen =
					RectTransformUtility.WorldToScreenPoint(live.GraphicRaycaster.eventCamera, clickWorld);
				PointerEventData pointer = new(EventSystem.current) { position = clickScreen };
				List<RaycastResult> hits = new();
				EventSystem.current.RaycastAll(pointer, hits);
				Require(hits.Count > 0 && hits[0].gameObject == clickable.gameObject,
					$"Duck pointer hit: {hits.Count} hits");
				report.AppendLine("PASS duck area receives pointer raycast");
				await ((IWindowControl)live).Close(false, default);
				live.Canvas.enabled = false;
				live.GraphicRaycaster.enabled = false;
				using GameplayPlaytestFixture fixture = new(scene.Container);
				GameContext game = fixture.Game;
				fixture.Run.ReplaceAnswersCopied(7);
				fixture.Run.ReplaceExamElapsedSeconds(76f);
				fixture.Run.ReplaceSuspicionLevel(85f);
				fixture.Run.isBellAnnounced = true;
				GameEntity duck = fixture.Container.Resolve<IDuckFactory>().CreateDuck();
				GameEntity teacher = fixture.Container.Resolve<ITeacherFactory>().CreateTeacher();
				GameEntity neighbour = fixture.Container.Resolve<INeighbourFactory>()
					.CreateNeighbour(NeighbourSide.Left);
				fixture.Container.Instantiate<InitializeMeowSourceSystem>().Initialize();
				AttendancePlaytestMicrophone microphone =
					new(fixture.Container.Resolve<IMeowConfigsService>().MeowConfig);
				ExamQuery exam = fixture.Container.Instantiate<ExamQuery>();
				BellQuery bell = fixture.Container.Instantiate<BellQuery>();
				SuspicionQuery suspicion = fixture.Container.Instantiate<SuspicionQuery>();
				MeowQuery meow = new(game, microphone, microphone);
				DuckQuery ducks = new(game);
				TeacherQuery teachers = new(game);
				NeighbourQuery neighbours = fixture.Container.Instantiate<NeighbourQuery>();
				ring = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(
					"Assets/AddressableResources/Content/Papers/PawTimer.prefab"));
				ring.transform.position = new Vector3(-1000f, 0f, 0f);
				instance = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(
					"Assets/AddressableResources/Content/UI/Gameplay/GameplayWindow.prefab"));
				instance.transform.SetParent(live.transform.parent, false);
				GameplayWindow window = instance.GetComponent<GameplayWindow>();
				control = window;
				window.Construct(exam, bell, suspicion, meow, ducks,
					fixture.Container.Resolve<IDuckFactory>(), teachers, neighbours);
				await window.Initialize("Overlay", "hud-playtest");
				await control.Open(false, default);

				window.Canvas.enabled = false;
				SerializedObject fields = new(window);
				Require(Field<TMP_Text>(fields, "answers").text == "ANSWERS 7 / 12", "Seed answers");
				Require(Field<TMP_Text>(fields, "clock").text == "0:44", "Seed clock");
				Require(Field<TMP_Text>(fields, "clock").color.r > 0.7f, "Announced clock red on reopen");
				Require(Mathf.Approximately(Field<Image>(fields, "suspicionFill").fillAmount, 0.85f), "Seed suspicion");
				report.AppendLine("PASS mid-session seed: answers, timer, announced color and suspicion");
				fixture.Run.ReplaceAnswersCopied(8);
				exam.ReactToChanges();
				Require(Field<TMP_Text>(fields, "answers").text == "ANSWERS 8 / 12", "Reactive answers");
				fixture.Run.ReplaceExamElapsedSeconds(120f);
				bell.ReactToChanges();
				Require(Field<TMP_Text>(fields, "clock").text == "0:00", "Clock clamps");
				report.AppendLine("PASS reactive answers and clock at zero");
				GameEntity source = game.GetGroup(GameMatcher
					.AllOf(
						GameMatcher.MeowSource)).GetSingleEntity();
				source.ReplaceMicrophoneLevel(70f);
				source.AddCooldownTimeLeft(microphone.MeowConfig.CooldownSeconds * 0.5f);
				source.isOnCooldown = true;
				source.isMeowArmed = false;
				meow.ReactToChanges();
				Require(Mathf.Approximately(Field<Image>(fields, "microphoneFill").fillAmount, 0.7f), "Mic level");
				Require(Mathf.Approximately(Field<Image>(fields, "cooldownFill").fillAmount, 0.5f), "Cooldown");
				float threshold = microphone.MeowConfig.ThresholdLevel / 100f;
				Require(Mathf.Approximately(Field<RectTransform>(fields, "microphoneThreshold").anchoredPosition.y,
					-216f + 192f * threshold), "Config threshold");
				microphone.IsAvailable = false;
				meow.ReactToChanges();
				Require(Field<TMP_Text>(fields, "microphoneHint").text == "No mic — press M to meow", "Fallback");
				report.AppendLine("PASS microphone fill, config threshold, cooldown, missing-device fallback");
				Button button = Field<Button>(fields, "duckButton");
				button.onClick.Invoke();
				Require(game.GetGroup(GameMatcher
					.AllOf(
						GameMatcher.ThrowDuckRequest)).count == 1, "Duck request");
				duck.ReplaceDuckState(DuckState.Flying);
				ducks.ReactToChanges();
				Require(button.interactable == false, "Flying duck disabled");
				duck.ReplaceDuckState(DuckState.Confiscated);
				ducks.ReactToChanges();
				Require(Field<TMP_Text>(fields, "speech").text == "That's it. The duck is MINE.", "Confiscation line");
				report.AppendLine("PASS Q click request, unavailable duck and confiscation line");
				neighbour.isPawLifted = true;
				float duration = fixture.Container.Resolve<IDifficultyService>().GetPhase(0).PawWindow;
				neighbour.ReplacePawWindowTimeLeft(duration * 0.5f);
				neighbours.ReactToChanges();
				Canvas canvas = ring.GetComponentInChildren<Canvas>();
				Image fill = canvas.transform.Find("ring_timer").GetComponent<Image>();
				Require(canvas.enabled && Mathf.Approximately(fill.fillAmount, 0.5f), "World ring progress");
				neighbour.isPawLifted = false;
				neighbour.ReplacePawWindowTimeLeft(0f);
				neighbours.ReactToChanges();
				Require(canvas.enabled == false && fill.fillAmount == 0f, "Expired ring");
				report.AppendLine("PASS world-space paw ring at half window and expiration");
				teacher.ReplaceTeacherAttention(TeacherAttention.Turning);
				teachers.ReactToChanges();
				Require(Field<TMP_Text>(fields, "speech").text == "Hmm?", "Telegraph");
				teacher.isTeacherAttentionChanged = false;
				fixture.Container.Resolve<IEntityFactory>().Event()
					.AddTeacherRemarkEvent(TeacherRemark.PencilAlert);
				new EventsReadySystem(game).Execute();
				teachers.ReactToChanges();
				Require(Field<TMP_Text>(fields, "speech").text == "What was that?!", "Pencil cause");
				await UniTask.Delay(TimeSpan.FromSeconds(1.3));
				Require(Field<GameObject>(fields, "bubble").activeSelf == false, "Bubble lifetime");
				report.AppendLine("PASS teacher telegraph, event cause and 1.2-second bubble lifetime");
				fixture.Run.ReplaceExamOutcome(ExamOutcome.Caught);
				fixture.Run.isExamFinished = true;
				exam.ReactToChanges();
				teachers.ReactToChanges();
				Require(Field<TMP_Text>(fields, "speech").text == "CAUGHT. See me after class.", "Outcome wins");
				await control.Close(false, default);
				fixture.Run.ReplaceAnswersCopied(9);
				exam.ReactToChanges();
				Require(Field<TMP_Text>(fields, "answers").text == "ANSWERS 8 / 12", "Unsubscribe");
				await control.Open(false, default);
				Require(Field<TMP_Text>(fields, "answers").text == "ANSWERS 9 / 12", "Reseed");
				await control.Close(false, default);
				report.AppendLine("PASS outcome priority, close unsubscription and reopen seed");
				report.AppendLine("DONE");
			}
			catch (Exception exception)
			{
				report.AppendLine("FAIL " + exception);
				Debug.LogException(exception);
			}
			finally
			{
				if (control != null)
					await control.Close(false, default);
				if (instance != null)
					Object.Destroy(instance);
				if (ring != null)
					Object.Destroy(ring);
				await UniTask.NextFrame();
				if (live != null)
				{
					live.Canvas.enabled = true;
					live.GraphicRaycaster.enabled = true;
					await ((IWindowControl)live).Open(false, default);
				}
				File.WriteAllText(PlaytestPaths.Get("hud.txt"), report.ToString());
			}
		}

		private static T Field<T>(SerializedObject fields, string name) where T : Object
		{
			return (T)fields.FindProperty(name).objectReferenceValue;
		}

		private static void Require(bool condition, string message)
		{
			if (condition == false)
				throw new InvalidOperationException(message);
		}
	}
}
