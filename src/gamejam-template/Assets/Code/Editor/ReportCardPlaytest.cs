using System;
using System.IO;
using System.Text;
using Code.Gameplay.Duck.Queries;
using Code.Gameplay.Duck.Services;
using Code.Gameplay.Exam;
using Code.Gameplay.Exam.Queries;
using Code.Gameplay.Exam.Services;
using Code.Gameplay.Leaderboard.Data;
using Code.Gameplay.Progress.Queries;
using Code.Gameplay.Progress.Services;
using Code.Gameplay.Teacher.Queries;
using Code.Gameplay.Teacher.Services;
using Code.Storage.SaveFiles;
using Code.UI.Fade;
using Code.UI.Gameplay;
using Code.UI.Result;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
using Framework.UI.UiManagement.Services;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace Code.Editor
{
	public static class ReportCardPlaytest
	{
		private const string Prefab = "Assets/AddressableResources/Content/UI/Result/ResultScreen.prefab";

		[MenuItem("COPYCAT/QA/Test report card")]
		public static void Run() => RunChecks().Forget();

		private static async UniTask RunChecks()
		{
			StringBuilder report = new();
			GameplayWindow live = Object.FindFirstObjectByType<GameplayWindow>();
			GameObject instance = null;
			IWindowControl control = null;
			try
			{
				if (EditorApplication.isPlaying == false || live == null)
					throw new InvalidOperationException("Enter the exam before running report card checks.");

				CheckGrades(report);

				SceneContext scene = null;
				foreach (SceneContext candidate in Object.FindObjectsByType<SceneContext>(FindObjectsSortMode.None))
				{
					if (candidate.gameObject.scene.name == "Gameplay")
						scene = candidate;
				}

				using GameplayPlaytestFixture fixture = new(scene.Container);
				GameEntity teacher = fixture.Container.Resolve<ITeacherFactory>().CreateTeacher();
				GameEntity duck = fixture.Container.Resolve<IDuckFactory>().CreateDuck();
				fixture.Container.Resolve<IProgressFactory>().CreateExamProgress(new GeneralSaveFile
				{
					PlayerName = "Mittens",
					BestAnswers = 11,
					BestTimeSeconds = 95f
				});
				fixture.Run.ReplaceAnswersCopied(12);
				fixture.Run.ReplaceExamElapsedSeconds(134f);
				fixture.Run.ReplaceMeowCount(8);
				fixture.Run.ReplaceExamOutcome(ExamOutcome.Passed);
				fixture.Run.isExamFinished = true;
				teacher.ReplaceAlmostCaughtCount(1);
				duck.ReplaceDuckThrowCount(0);

				ExamQuery exam = fixture.Container.Instantiate<ExamQuery>();
				TeacherQuery teachers = new(fixture.Game);
				DuckQuery ducks = fixture.Container.Instantiate<DuckQuery>();
				ProgressQuery progress = new(fixture.Game);
				PlaytestLeaderboard leaderboard = new() { IsPending = true };
				PlaytestCoreLoop coreLoop = new();
				PlaytestInput input = new();
				IUiService uiService = ProjectContext.Instance.Container.Resolve<IUiService>();

				instance = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Prefab));
				instance.transform.SetParent(live.transform.parent, false);
				scene.Container.InjectGameObject(instance);
				ResultWindow window = instance.GetComponent<ResultWindow>();
				control = window;
				window.Construct(exam, teachers, ducks, progress, new ExamGradeService(), leaderboard, input,
					coreLoop, coreLoop);
				await window.Initialize("Overlay", "report-card-playtest");
				await control.Open(false, default);
				window.Canvas.enabled = false;

				SerializedObject fields = new(window);
				Require(Field<TMP_Text>(fields, "title").text == "EXAM PASSED!", "Passed title");
				Require(Field<TMP_Text>(fields, "subtitle").text == "...You all passed? Suspicious.", "Passed line");
				Require(Values(fields)[0].text == "12/12" && Values(fields)[1].text == "2:14"
					&& Values(fields)[2].text == "8" && Values(fields)[3].text == "1"
					&& Values(fields)[4].text == "0", "Stats");
				Require(Field<Image>(fields, "gradeStamp").sprite.name == "stamp_grade_A+", "A+ stamp");
				Require(Field<TMP_Text>(fields, "gradeMessage").text == "Purrfect crime. No duck, no evidence.",
					"Three star message");
				Require(FilledStars(fields) == 3, "Three stars filled");
				Require(Field<TMP_Text>(fields, "personalBest").text == "Your best: 11 answers · 1:35", "Personal best");
				report.AppendLine("PASS passed outcome: title, line, five stats, A+ stamp, stars and personal best");

				Require(Field<TMP_Text>(fields, "leaderboardStatus").text == "Sending your result…", "Sending");
				Require(Rows(fields)[0].gameObject.activeSelf == false, "Rows hidden while sending");
				Require(leaderboard.SubmitCount == 1 && leaderboard.Submitted.Name == "Mittens"
					&& leaderboard.Submitted.Answers == 12 && leaderboard.Submitted.Grade == "A+"
					&& Mathf.Approximately(leaderboard.Submitted.TimeSeconds, 134f), "Submitted entry");
				leaderboard.Response = PlaytestLeaderboard.BuildResponse(rank: 3, total: 20);
				leaderboard.IsPending = false;
				await UniTask.NextFrame();
				await UniTask.NextFrame();
				Require(Field<TMP_Text>(fields, "leaderboardStatus").gameObject.activeSelf == false, "Status hidden");
				Require(Rows(fields)[0].gameObject.activeSelf && Rows(fields)[9].gameObject.activeSelf, "Ten rows");
				Require(Highlighted(fields) == 3, "Own row highlighted");
				Require(Field<TMP_Text>(fields, "ownRank").gameObject.activeSelf == false, "Own rank line hidden");
				report.AppendLine("PASS submitted entry, ten rows, highlighted own rank and hidden rank line");

				await control.Close(false, default);
				leaderboard.Response = PlaytestLeaderboard.BuildResponse(rank: 14, total: 20);
				await control.Open(false, default);
				window.Canvas.enabled = false;
				await UniTask.NextFrame();
				await UniTask.NextFrame();
				Require(Field<TMP_Text>(fields, "ownRank").gameObject.activeSelf
					&& Field<TMP_Text>(fields, "ownRank").text == "#14 — you", "Rank outside top ten");
				await control.Close(false, default);
				report.AppendLine("PASS rank outside the top ten shows the own line");

				leaderboard.Response = LeaderboardResponse.Offline;
				await control.Open(false, default);
				window.Canvas.enabled = false;
				await UniTask.NextFrame();
				await UniTask.NextFrame();
				Require(Field<TMP_Text>(fields, "leaderboardStatus").text == "Leaderboard offline", "Offline");
				Require(Rows(fields)[0].gameObject.activeSelf == false, "Rows hidden offline");
				await control.Close(false, default);
				report.AppendLine("PASS offline response keeps rows hidden behind the offline caption");

				fixture.Run.ReplaceAnswersCopied(7);
				fixture.Run.ReplaceExamOutcome(ExamOutcome.Caught);
				teacher.ReplaceAlmostCaughtCount(4);
				duck.ReplaceDuckThrowCount(3);
				await control.Open(false, default);
				window.Canvas.enabled = false;
				Require(Field<TMP_Text>(fields, "title").text == "CAUGHT"
					&& Field<TMP_Text>(fields, "subtitle").text == "CAUGHT. See me after class.", "Caught header");
				Require(Field<Image>(fields, "gradeStamp").sprite.name == "stamp_grade_C"
					&& Field<TMP_Text>(fields, "gradeMessage").text == "Average copycat. Whiskerstein noticed."
					&& FilledStars(fields) == 0 && Stars(fields)[0].gameObject.activeSelf == false, "C grade");
				await control.Close(false, default);
				fixture.Run.ReplaceExamOutcome(ExamOutcome.BellRang);
				await control.Open(false, default);
				window.Canvas.enabled = false;
				Require(Field<TMP_Text>(fields, "title").text == "BELL RANG"
					&& Field<TMP_Text>(fields, "subtitle").text == "RIIING! Pencils down!", "Bell header");
				report.AppendLine("PASS caught and bell headers, C stamp, its message and hidden stars");

				Button retake = Field<Button>(fields, "retakeButton");
				Button menu = Field<Button>(fields, "menuButton");
				input.PressedKey = KeyCode.R;
				await UniTask.NextFrame();
				await UniTask.NextFrame();
				Require(retake.interactable == false && menu.interactable == false, "R starts the retake");
				input.PressedKey = KeyCode.None;
				await control.Close(false, default);
				coreLoop.Calls.Clear();
				await control.Open(false, default);
				window.Canvas.enabled = false;
				menu.onClick.Invoke();
				Require(retake.interactable == false, "Menu locks the buttons");
				await WaitForTransition(uiService);
				Require(coreLoop.Calls.Contains("camera:StartLaunch") && coreLoop.Calls.Contains("close:Exam")
					&& coreLoop.Calls.Contains("node:StartLaunch"), "Menu transition");
				await control.Close(false, default);
				coreLoop.Calls.Clear();
				await control.Open(false, default);
				window.Canvas.enabled = false;
				retake.onClick.Invoke();
				await WaitForTransition(uiService);
				Require(coreLoop.Calls.Contains("camera:Exam") && coreLoop.Calls.Contains("close:Exam")
					&& coreLoop.Calls.Contains("branch:Exam"), "Retake transition");
				report.AppendLine("PASS R key, MAIN MENU node request and RETAKE EXAM branch restart");

				await control.Close(false, default);
				Require(leaderboard.SubmitCount == 7, "One submit per open");
				report.AppendLine("PASS exactly one leaderboard submit per report card");
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
				await UniTask.NextFrame();
				if (EditorApplication.isPlaying)
					await ProjectContext.Instance.Container.Resolve<IUiService>().CloseWindow<FadeWindow>();
				File.WriteAllText(PlaytestPaths.Get("report-card.txt"), report.ToString());
			}
		}

		private static async UniTask WaitForTransition(IUiService uiService)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(1.2), DelayType.UnscaledDeltaTime);
			await uiService.CloseWindow<FadeWindow>();
		}

		private static void CheckGrades(StringBuilder report)
		{
			ExamGradeService grades = new();
			ExamGrade[] expected =
			{
				ExamGrade.F, ExamGrade.F, ExamGrade.F,
				ExamGrade.D, ExamGrade.D, ExamGrade.D,
				ExamGrade.C, ExamGrade.C, ExamGrade.C,
				ExamGrade.B, ExamGrade.B,
				ExamGrade.A,
				ExamGrade.APlus
			};

			for (int answers = 0; answers < expected.Length; answers++)
				Require(grades.GetGrade(answers) == expected[answers], "Grade for " + answers);

			Require(grades.GetStars(12, 0, 0) == 3 && grades.GetStars(12, 0, 1) == 3, "Three stars");
			Require(grades.GetStars(12, 1, 0) == 2 && grades.GetStars(12, 0, 3) == 2, "Two stars");
			Require(grades.GetStars(12, 0, 4) == 1 && grades.GetStars(12, 3, 9) == 1, "One star");
			Require(grades.GetStars(11, 0, 0) == 0, "No stars below twelve");
			report.AppendLine("PASS grade table for 0-12 answers and the three star tiers");
		}

		private static int FilledStars(SerializedObject fields)
		{
			int filled = 0;

			foreach (Image star in Stars(fields))
			{
				if (star.gameObject.activeSelf && star.sprite.name == "star_filled")
					filled++;
			}

			return filled;
		}

		private static int Highlighted(SerializedObject fields)
		{
			ResultLeaderboardRow[] rows = Rows(fields);

			for (int index = 0; index < rows.Length; index++)
			{
				if (rows[index].GetComponentInChildren<Image>(true).enabled)
					return index + 1;
			}

			return 0;
		}

		private static Image[] Stars(SerializedObject fields) => Items<Image>(fields, "stars");

		private static TMP_Text[] Values(SerializedObject fields) => Items<TMP_Text>(fields, "statValues");

		private static ResultLeaderboardRow[] Rows(SerializedObject fields)
		{
			return Items<ResultLeaderboardRow>(fields, "leaderboardRows");
		}

		private static T[] Items<T>(SerializedObject fields, string name) where T : Object
		{
			SerializedProperty property = fields.FindProperty(name);
			T[] values = new T[property.arraySize];

			for (int index = 0; index < values.Length; index++)
				values[index] = (T)property.GetArrayElementAtIndex(index).objectReferenceValue;

			return values;
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
