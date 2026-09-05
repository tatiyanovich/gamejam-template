using System;
using System.IO;
using System.Text;
using Code.Gameplay.Neighbours;
using Code.Gameplay.Neighbours.Behaviours;
using Code.Gameplay.Neighbours.Queries;
using Code.Gameplay.Neighbours.Services;
using Code.Gameplay.Teacher;
using Code.Gameplay.Teacher.Queries;
using Code.Gameplay.Teacher.Services;
using Code.UI.Gameplay;
using Code.UI.Launch;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace Code.Editor
{
	public static class NeighbourAnimationPlaytest
	{
		[MenuItem("COPYCAT/QA/Test neighbour animation")]
		public static void Run() => RunChecks().Forget();

		private static async UniTask RunChecks()
		{
			StringBuilder report = new();
			GameObject leftInstance = null;
			GameObject rightInstance = null;
			GameObject timerInstance = null;
			GameObject leftSource = null;
			GameObject rightSource = null;
			GameObject timerSource = null;
			try
			{
				if (EditorApplication.isPlaying == false)
					throw new InvalidOperationException("Enter Play Mode before running neighbour animation checks.");

				SceneContext scene = await FindOrEnterGameplayScene();
				await UniTask.Delay(TimeSpan.FromSeconds(0.5));
				using GameplayPlaytestFixture fixture = new(scene.Container);
				fixture.Run.ReplaceCurrentQuestionIndex(0);
				INeighbourFactory neighbourFactory = fixture.Container.Resolve<INeighbourFactory>();
				GameEntity left = neighbourFactory.CreateNeighbour(NeighbourSide.Left);
				GameEntity right = neighbourFactory.CreateNeighbour(NeighbourSide.Right);
				GameEntity teacher = fixture.Container.Resolve<ITeacherFactory>().CreateTeacher();
				NeighbourQuery neighbourQuery = fixture.Container.Instantiate<NeighbourQuery>();
				TeacherQuery teacherQuery = new(fixture.Game);

				leftSource = FindWorldView<NeighbourView>(NeighbourSide.Left).gameObject;
				rightSource = FindWorldView<NeighbourView>(NeighbourSide.Right).gameObject;
				timerSource = FindWorldView<PawTimerView>(NeighbourSide.Left).gameObject;
				leftInstance = CreateCharacter("Whiskerstein", leftSource.transform);
				rightInstance = CreateCharacter("Fluffy", rightSource.transform);
				timerInstance = CreateInstance(
					AssetDatabase.LoadAssetAtPath<GameObject>(
						"Assets/AddressableResources/Content/Papers/PawTimer.prefab"),
					timerSource.transform);
				leftSource.SetActive(false);
				rightSource.SetActive(false);
				timerSource.SetActive(false);
				NeighbourView leftView = leftInstance.GetComponent<NeighbourView>();
				NeighbourView rightView = rightInstance.GetComponent<NeighbourView>();
				leftView.Bind(neighbourQuery, teacherQuery);
				rightView.Bind(neighbourQuery, teacherQuery);

				PawTimerView timer = timerInstance.GetComponent<PawTimerView>();
				timer.Bind(neighbourQuery);

				Transform leftPaw = leftView.transform.Find("Paw");
				Transform rightPaw = rightView.transform.Find("Paw");
				Transform leftHead = leftView.transform.Find("nerd_head");
				Vector3 pawOrigin = leftPaw.localPosition;
				Vector3 rightPawOrigin = rightPaw.localPosition;
				Vector3 headOrigin = leftHead.localPosition;
				float headTravel = 0f;

				left.isPawLifted = true;
				left.ReplacePawWindowTimeLeft(10f);
				neighbourQuery.ReactToChanges();
				for (int frame = 0; frame < 20; frame++)
				{
					await UniTask.NextFrame();
					headTravel = Mathf.Max(headTravel, Vector3.Distance(leftHead.localPosition, headOrigin));
				}
				await UniTask.Delay(TimeSpan.FromSeconds(0.3));
				Require(leftPaw.localPosition.y > pawOrigin.y + 0.35f, "Left paw lift offset");
				Require(Mathf.Abs(Mathf.DeltaAngle(leftPaw.localEulerAngles.z, 70f)) < 1f, "Left paw lift angle");
				Require(headTravel > 0.02f, "Head jolt on meow");
				Require(Mathf.Approximately(rightPaw.localPosition.y, rightPawOrigin.y), "Wrong neighbour moved");
				Require(timerInstance.GetComponentInChildren<Canvas>().enabled, "Paw timer hidden");
				Require(timerInstance.transform.Find("Canvas/ring_timer").GetComponent<Image>().fillAmount > 0.99f,
					"Paw timer fill");
				ScreenCapture.CaptureScreenshot(PlaytestPaths.Get("neighbour-lifted.png"));
				await UniTask.DelayFrame(2);
				report.AppendLine("PASS lift: left paw moves 0.4 units and 70 degrees from the shoulder");

				left.ReplacePawWindowTimeLeft(5f);
				neighbourQuery.ReactToChanges();
				left.ReplacePawWindowTimeLeft(10f);
				neighbourQuery.ReactToChanges();
				headTravel = 0f;
				for (int frame = 0; frame < 20; frame++)
				{
					await UniTask.NextFrame();
					headTravel = Mathf.Max(headTravel, Vector3.Distance(leftHead.localPosition, headOrigin));
				}
				Require(headTravel > 0.02f, "Head jolt on repeated meow");
				Require(timerInstance.transform.Find("Canvas/ring_timer").GetComponent<Image>().fillAmount > 0.99f,
					"Restarted paw timer fill");
				report.AppendLine("PASS meow: first and repeated meows jolt the matching neighbour and restart the ring");

				left.ReplacePawWindowTimeLeft(0.8f);
				neighbourQuery.ReactToChanges();
				float minimumAngle = 360f;
				float maximumAngle = 0f;
				for (int frame = 0; frame < 20; frame++)
				{
					await UniTask.NextFrame();
					float angle = Mathf.DeltaAngle(0f, leftPaw.localEulerAngles.z);
					minimumAngle = Mathf.Min(minimumAngle, angle);
					maximumAngle = Mathf.Max(maximumAngle, angle);
				}
				Require(maximumAngle - minimumAngle > 2f, "Last-second paw telegraph");
				Require(timerInstance.transform.Find("Canvas/ring_timer").GetComponent<Image>().fillAmount < 0.1f,
					"Paw timer countdown");
				report.AppendLine("PASS telegraph: the last second shakes the paw while the radial ring drains");

				teacher.SwitchAttention(TeacherAttention.Watching, 10f);
				teacherQuery.ReactToChanges();
				await UniTask.Delay(TimeSpan.FromSeconds(0.15));
				Require(leftHead.localPosition.y < headOrigin.y - 0.15f, "Neighbour looks down under teacher gaze");
				teacher.SwitchAttention(TeacherAttention.Writing, 10f);
				teacherQuery.ReactToChanges();
				await UniTask.Delay(TimeSpan.FromSeconds(0.15));
				Require(Mathf.Abs(leftHead.localPosition.y - headOrigin.y) < 0.01f, "Neighbour head return");
				report.AppendLine("PASS gaze: neighbours duck into their papers while Mrs. Hisskins watches");

				left.isPawLifted = false;
				left.ReplacePawWindowTimeLeft(0f);
				neighbourQuery.ReactToChanges();
				await UniTask.Delay(TimeSpan.FromSeconds(0.4));
				Require(Vector3.Distance(leftPaw.localPosition, pawOrigin) < 0.01f, "Left paw cover position");
				Require(Mathf.Abs(Mathf.DeltaAngle(leftPaw.localEulerAngles.z, 0f)) < 1f, "Left paw cover angle");
				Require(timerInstance.GetComponentInChildren<Canvas>().enabled == false, "Paw timer remains visible");
				ScreenCapture.CaptureScreenshot(PlaytestPaths.Get("neighbour-covered.png"));
				await UniTask.DelayFrame(2);
				report.AppendLine("PASS cover: paw returns over the answer in 0.35 seconds and hides the ring");

				right.isPawLifted = true;
				right.ReplacePawWindowTimeLeft(10f);
				neighbourQuery.ReactToChanges();
				await UniTask.Delay(TimeSpan.FromSeconds(0.3));
				Require(rightPaw.localPosition.y > rightPawOrigin.y + 0.35f, "Right paw lift offset");
				Require(Mathf.Abs(Mathf.DeltaAngle(rightPaw.localEulerAngles.z, 70f)) < 1f, "Right paw lift angle");
				report.AppendLine("PASS mirror: Fluffy uses the same authored shoulder motion under a mirrored root");
				report.AppendLine("DONE");
			}
			catch (Exception exception)
			{
				report.AppendLine("FAIL " + exception);
				Debug.LogException(exception);
			}
			finally
			{
				if (leftInstance != null)
					Object.Destroy(leftInstance);

				if (rightInstance != null)
					Object.Destroy(rightInstance);

				if (timerInstance != null)
					Object.Destroy(timerInstance);

				if (leftSource != null)
					leftSource.SetActive(true);

				if (rightSource != null)
					rightSource.SetActive(true);

				if (timerSource != null)
					timerSource.SetActive(true);

				File.WriteAllText(PlaytestPaths.Get("neighbour.txt"), report.ToString());
			}
		}

		private static GameObject CreateCharacter(string name, Transform source)
		{
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
				$"Assets/AddressableResources/Content/Characters/Neighbours/{name}.prefab");
			return CreateInstance(prefab, source);
		}

		private static GameObject CreateInstance(GameObject prefab, Transform source)
		{
			GameObject instance = Object.Instantiate(prefab, source.parent);
			instance.transform.localPosition = source.localPosition;
			instance.transform.localRotation = source.localRotation;
			instance.transform.localScale = source.localScale;
			return instance;
		}

		private static T FindWorldView<T>(NeighbourSide side) where T : Component
		{
			foreach (T view in Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None))
			{
				bool matches = side == NeighbourSide.Left ? view.transform.position.x < 0f
					: view.transform.position.x > 0f;
				if (matches)
					return view;
			}

			throw new InvalidOperationException($"World view not found: {typeof(T).Name} {side}");
		}

		private static async UniTask<SceneContext> FindOrEnterGameplayScene()
		{
			SceneContext scene = FindGameplayScene();
			if (scene != null)
				return scene;

			LaunchWindow launch = Object.FindFirstObjectByType<LaunchWindow>();
			if (launch == null)
				throw new InvalidOperationException("Enter Play Mode before running neighbour animation checks.");

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

		private static void Require(bool condition, string message)
		{
			if (condition == false)
				throw new InvalidOperationException(message);
		}
	}
}
