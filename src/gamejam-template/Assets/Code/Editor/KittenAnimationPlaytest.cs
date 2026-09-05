using System;
using System.IO;
using System.Text;
using Code.Gameplay.Exam.Data;
using Code.Gameplay.Exam.Queries;
using Code.Gameplay.Exam.Services;
using Code.Gameplay.Input.Behaviours;
using Code.Gameplay.Input.Queries;
using Code.Gameplay.Neighbours;
using Code.Gameplay.Teacher;
using Code.Gameplay.Teacher.Queries;
using Code.Gameplay.Teacher.Services;
using Code.UI.Launch;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace Code.Editor
{
	public static class KittenAnimationPlaytest
	{
		[MenuItem("COPYCAT/QA/Test kitten animation")]
		public static void Run() => RunChecks().Forget();

		private static async UniTask RunChecks()
		{
			StringBuilder report = new();
			GameObject instance = null;
			float timeScale = Time.timeScale;
			try
			{
				if (EditorApplication.isPlaying == false)
					throw new InvalidOperationException("Enter the exam before running kitten animation checks.");

				SceneContext scene = await FindOrEnterGameplayScene();
				await UniTask.Delay(TimeSpan.FromSeconds(0.5));
				using GameplayPlaytestFixture fixture = new(scene.Container);
				GameEntity question = fixture.Exams.CreateQuestion(0);
				GameEntity teacher = fixture.Container.Resolve<ITeacherFactory>().CreateTeacher();
				fixture.Run.isCurrentQuestionIndexChanged = false;
				question.isAnswerProgressChanged = false;
				InputQuery inputQuery = new(fixture.Input);
				ExamQuery examQuery = fixture.Container.Instantiate<ExamQuery>();
				TeacherQuery teacherQuery = new(fixture.Game);
				instance = Object.Instantiate(
					AssetDatabase.LoadAssetAtPath<GameObject>(
						"Assets/AddressableResources/Content/Characters/Kitten/Kitten.prefab"),
					scene.transform);
				instance.SetActive(false);
				KittenView view = instance.GetComponent<KittenView>();
				view.Bind(inputQuery, examQuery, teacherQuery);

				Transform idle = Pose(view, "idle");
				Transform idleTail = idle.Find("tail");
				float initialScale = idle.localScale.x;
				float initialTail = idleTail.localEulerAngles.z;
				await UniTask.Delay(TimeSpan.FromSeconds(0.3));
				Require(Active(view, "idle"), "Idle pose");
				Require(Mathf.Abs(idle.localScale.x - initialScale) > 0.002f, "Idle breathing");
				Require(Mathf.Abs(Mathf.DeltaAngle(initialTail, idleTail.localEulerAngles.z)) > 2f, "Idle tail");
				report.AppendLine("PASS idle: body breathes and tail sways on the authored loop");

				fixture.Keyboard.isLeanHeld = true;
				inputQuery.ReactToChanges();
				await UniTask.Delay(TimeSpan.FromSeconds(0.25));
				Transform leanLeft = Pose(view, "lean_left");
				Require(Active(view, "lean_left"), "Left lean pose");
				Require(leanLeft.localPosition.x < -0.55f, "Left lean shift");
				Require(Mathf.Abs(Mathf.DeltaAngle(leanLeft.localEulerAngles.z, 0f)) > 10f, "Left lean rotation");
				report.AppendLine("PASS lean left: profile shifts 0.6 units and rotates to the neighbour");

				teacher.SwitchAttention(TeacherAttention.Watching, 10f);
				teacherQuery.ReactToChanges();
				Transform panic = Pose(view, "panic_left");
				Transform panicHead = panic.Find("Head");
				Require(Active(view, "panic_left"), "Panic pose");
				Require(panicHead.Find("kitten_eye_white").localScale.x > 1.15f, "Panic eyes");
				Require(panicHead.Find("kitten_pupil").localScale.x < 0.65f, "Panic pupils");
				report.AppendLine("PASS panic: Watching swaps the active lean to wide eyes and small pupils");

				Transform paw = panic.Find("pawRight");
				float restingPaw = paw.localEulerAngles.z;
				Time.timeScale = 0.1f;
				question.ReplaceAnswerProgress(1);
				examQuery.ReactToChanges();
				float pawTravel = 0f;
				for (int frame = 0; frame < 120 && pawTravel <= 3f; frame++)
				{
					await UniTask.NextFrame();
					pawTravel = Mathf.Abs(Mathf.DeltaAngle(restingPaw, paw.localEulerAngles.z));
				}
				Require(pawTravel > 3f, "Typing paw");
				await UniTask.Delay(TimeSpan.FromSeconds(0.12));
				Require(Mathf.Abs(Mathf.DeltaAngle(restingPaw, paw.localEulerAngles.z)) < 1f, "Typing paw return");
				Time.timeScale = timeScale;
				report.AppendLine("PASS typing: every correct progress step taps the right paw and returns it");

				int rightQuestionIndex = FindQuestion(
					NeighbourSide.Right,
					fixture.Container.Resolve<IExamConfigsService>());
				fixture.Exams.CreateQuestion(rightQuestionIndex);
				fixture.Run.ReplaceCurrentQuestionIndex(rightQuestionIndex);
				examQuery.ReactToChanges();
				teacher.SwitchAttention(TeacherAttention.Writing, 10f);
				teacherQuery.ReactToChanges();
				await UniTask.Delay(TimeSpan.FromSeconds(0.25));
				Transform leanRight = Pose(view, "lean_right");
				Require(Active(view, "lean_right"), "Right lean pose");
				Require(leanRight.localPosition.x > 0.55f, "Right lean shift");
				Require(Mathf.Abs(Mathf.DeltaAngle(leanRight.localEulerAngles.z, 0f)) > 10f, "Right lean rotation");
				report.AppendLine("PASS lean right: the current question selects the matching neighbour profile");

				fixture.Keyboard.isLeanHeld = false;
				inputQuery.ReactToChanges();
				Require(Active(view, "idle"), "Return to idle");
				report.AppendLine("PASS release: letting go of SPACE restores the back-facing idle pose");
				report.AppendLine("DONE");
			}
			catch (Exception exception)
			{
				report.AppendLine("FAIL " + exception);
				Debug.LogException(exception);
			}
			finally
			{
				Time.timeScale = timeScale;
				if (instance != null)
					Object.Destroy(instance);

				File.WriteAllText(PlaytestPaths.Get("kitten.txt"), report.ToString());
			}
		}

		private static async UniTask<SceneContext> FindOrEnterGameplayScene()
		{
			SceneContext scene = FindGameplayScene();
			if (scene != null)
				return scene;

			LaunchWindow launch = Object.FindFirstObjectByType<LaunchWindow>();
			if (launch == null)
				throw new InvalidOperationException("Enter Play Mode before running kitten animation checks.");

			foreach (Button button in launch.GetComponentsInChildren<Button>(true))
			{
				if (button.name == "PLAY")
				{
					button.onClick.Invoke();
					break;
				}
			}

			for (int frame = 0; frame < 600; frame++)
			{
				await UniTask.Yield();
				scene = FindGameplayScene();
				if (scene != null)
					return scene;
			}

			throw new InvalidOperationException("Gameplay SceneContext did not load after PLAY.");
		}

		private static SceneContext FindGameplayScene()
		{
			foreach (SceneContext scene in Object.FindObjectsByType<SceneContext>(FindObjectsSortMode.None))
			{
				if (scene.gameObject.scene.name == "Gameplay")
					return scene;
			}

			return null;
		}

		private static int FindQuestion(NeighbourSide side, IExamConfigsService examConfigsService)
		{
			for (int index = 0; index < examConfigsService.ExamConfig.Questions.Count; index++)
			{
				QuestionDefinition question = examConfigsService.ExamConfig.Questions[index];
				if (question.Neighbour == side)
					return index;
			}

			throw new InvalidOperationException("Question side not found: " + side);
		}

		private static Transform Pose(KittenView view, string name) => view.transform.Find(name);

		private static bool Active(KittenView view, string name) => Pose(view, name).gameObject.activeSelf;

		private static void Require(bool condition, string message)
		{
			if (condition == false)
				throw new InvalidOperationException(message);
		}
	}
}
