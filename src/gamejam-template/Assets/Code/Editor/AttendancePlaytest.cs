using System;
using System.IO;
using System.Text;
using Code.Gameplay.Meow.Configs;
using Code.Gameplay.Meow.Queries;
using Code.Gameplay.Meow.Systems;
using Code.Infrastructure.CoreLoop;
using Code.Infrastructure.EntityComponentSystem.Events.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;
using Code.UI.Attendance;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Code.Editor
{
	public static class AttendancePlaytest
	{
		[MenuItem("COPYCAT/QA/Test attendance")]
		public static void Run()
		{
			if (EditorApplication.isPlaying == false)
				throw new InvalidOperationException("Run attendance checks in Play Mode.");

			RunChecks().Forget();
		}

		private static async UniTask RunChecks()
		{
			StringBuilder report = new();
			GameContext game = new();
			InputContext input = new();
			MeowConfig config = ScriptableObject.CreateInstance<MeowConfig>();
			GameObject instance = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(
				"Assets/AddressableResources/Content/UI/Attendance/AttendanceWindow.prefab"));
			AttendanceWindow window = instance.GetComponent<AttendanceWindow>();
			IWindowControl control = window;
			try
			{
				AttendancePlaytestMicrophone microphone = new(config);
				MeowQuery query = new(game, microphone, microphone);
				EntityFactory entities = new(game, input, new LoopNodeContext());
				new InitializeMeowSourceSystem(game, entities, new IdentifierService()).Initialize();
				SampleMicrophoneLevelSystem sample = new(game, microphone, microphone);
				EmitMeowOnLoudMicrophoneSystem emit = new(game, entities, microphone);
				window.Construct(null, query, microphone, null, null);
				await window.Initialize("Overlay", "attendance-playtest");
				await control.Open(false, default);
				window.Canvas.enabled = false;
				SerializedObject serialized = new(window);
				TMP_InputField name = (TMP_InputField)serialized.FindProperty("studentName").objectReferenceValue;
				TMP_Text hint = (TMP_Text)serialized.FindProperty("microphoneHint").objectReferenceValue;
				Image fill = (Image)serialized.FindProperty("microphoneFill").objectReferenceValue;
				GameObject check = (GameObject)serialized.FindProperty("microphoneCheckmark").objectReferenceValue;
				RectTransform threshold = (RectTransform)serialized.FindProperty("microphoneThreshold").objectReferenceValue;
				Button start = (Button)serialized.FindProperty("startExamButton").objectReferenceValue;
				Require(name.characterLimit == 12 && name.onValidateInput("", 0, 'Ж') == '\0'
					&& name.onValidateInput("", 0, '<') == '\0' && name.onValidateInput("", 0, ' ') == '\0'
					&& name.onValidateInput("", 0, 'Z') == 'Z' && name.onValidateInput("", 0, '9') == '9', "Name filter");
				report.AppendLine("PASS name limit and ASCII filter");
				Require(Mathf.Approximately(threshold.anchorMin.x, config.ThresholdLevel / 100f), "Threshold");
				Require(hint.text == "Meow to test your mic" && start.interactable, "Initial state");
				microphone.Level = 30f;
				sample.Execute();
				query.ReactToChanges();
				Require(Mathf.Approximately(fill.fillAmount, 0.3f), "Meter");
				float deadline = Time.realtimeSinceStartup + 3f;
				while (hint.text != "LOUDER!" && Time.realtimeSinceStartup < deadline)
					await UniTask.NextFrame();
				Require(hint.text == "LOUDER!", "Sustained quiet input");
				report.AppendLine("PASS meter, config threshold and LOUDER after 0.5 seconds");
				microphone.Level = 0f;
				sample.Execute();
				query.ReactToChanges();
				Require(hint.text == "Meow to test your mic", "Silence resets hint");
				entities.Event()
					.AddMeowEvent(false);
				new EventsReadySystem(game).Execute();
				query.ReactToChanges();
				Require(check.activeSelf == false, "Keyboard must not pass microphone check");
				new EventsCleanupSystem(game).Cleanup();
				report.AppendLine("PASS keyboard meow does not pass microphone check");
				microphone.Level = config.ThresholdLevel;
				sample.Execute();
				emit.Execute();
				query.ReactToChanges();
				Require(check.activeSelf == false, "Events must wait for the next frame");
				new EventsReadySystem(game).Execute();
				microphone.Level = 0f;
				sample.Execute();
				query.ReactToChanges();
				Require(check.activeSelf && hint.text == "LOUD ENOUGH!", "Short loud pulse");
				new EventsCleanupSystem(game).Cleanup();
				report.AppendLine("PASS threshold pulse survives next-frame silence and latches success");
				await control.Close(false, default);
				microphone.IsAvailable = false;
				await control.Open(false, default);
				Require(check.activeSelf == false && fill.fillAmount == 0f && start.interactable
					&& hint.text == "No mic — press M to meow", "No microphone");
				report.AppendLine("PASS reopen resets success; missing microphone keeps START EXAM enabled");
				await control.Close(false, default);
				microphone.IsAvailable = true;
				microphone.Level = 70f;
				sample.Execute();
				query.ReactToChanges();
				Require(fill.fillAmount == 0f, "Closed window subscription");
				report.AppendLine("PASS closed window unsubscribes from query");
				report.AppendLine("DONE");
			}
			catch (Exception exception)
			{
				report.AppendLine("FAIL " + exception);
				Debug.LogException(exception);
			}
			finally
			{
				Object.Destroy(instance);
				Object.Destroy(config);
				game.DestroyAllEntities();
				input.DestroyAllEntities();
				File.WriteAllText(PlaytestPaths.Get("attendance.txt"), report.ToString());
			}
		}

		private static void Require(bool condition, string message)
		{
			if (condition == false)
				throw new InvalidOperationException(message);
		}
	}
}
