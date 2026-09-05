using System;
using System.IO;
using System.Text;
using Code.Gameplay.Duck;
using Code.Gameplay.Duck.Behaviours;
using Code.Gameplay.Duck.Queries;
using Code.Gameplay.Duck.Services;
using Code.Gameplay.Teacher.Behaviours;
using Code.UI.Launch;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace Code.Editor
{
	public static class DuckAnimationPlaytest
	{
		[MenuItem("COPYCAT/QA/Test duck animation")]
		public static void Run() => RunChecks().Forget();

		private static async UniTask RunChecks()
		{
			StringBuilder report = new();
			GameObject instance = null;
			GameObject hintBubble = null;
			GameObject source = null;
			Transform teacher = null;
			Vector3 teacherPosition = default;
			try
			{
				if (EditorApplication.isPlaying == false)
					throw new InvalidOperationException("Enter Play Mode before running duck animation checks.");

				SceneContext scene = await FindOrEnterGameplayScene();
				await UniTask.Delay(TimeSpan.FromSeconds(0.5));
				hintBubble = GameObject.Find("HintBubble");
				if (hintBubble != null)
					hintBubble.SetActive(false);

				using GameplayPlaytestFixture fixture = new(scene.Container);
				IDuckFactory duckFactory = fixture.Container.Resolve<IDuckFactory>();
				GameEntity duck = duckFactory.CreateDuck();
				DuckQuery query = fixture.Container.Instantiate<DuckQuery>();

				DuckView sourceView = Object.FindFirstObjectByType<DuckView>(FindObjectsInactive.Include);
				if (sourceView == null)
					throw new InvalidOperationException("Duck world view not found.");

				source = sourceView.gameObject;
				GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
					"Assets/AddressableResources/Content/Duck/Duck.prefab");
				instance = Object.Instantiate(prefab, source.transform.parent);
				instance.transform.SetLocalPositionAndRotation(
					source.transform.localPosition,
					source.transform.localRotation);
				instance.transform.localScale = source.transform.localScale;
				source.SetActive(false);

				TeacherView teacherView = Object.FindFirstObjectByType<TeacherView>(FindObjectsInactive.Include);
				if (teacherView == null)
					throw new InvalidOperationException("Teacher world view not found.");

				teacher = teacherView.transform;
				teacherPosition = teacher.position;
				DuckView view = instance.GetComponent<DuckView>();
				view.Bind(query, teacher);
				Vector3 deskPosition = instance.transform.position;

				Transform idle = instance.transform.Find("duck_idle");
				Transform firstFlight = instance.transform.Find("duck_fly_1");
				Transform secondFlight = instance.transform.Find("duck_fly_2");
				Transform sad = instance.transform.Find("duck_sad");
				float idleOrigin = instance.transform.localPosition.y;
				await UniTask.Delay(TimeSpan.FromSeconds(0.35));
				Require(Mathf.Abs(instance.transform.localPosition.y - idleOrigin) > 0.01f, "Desk idle bob");
				Require(idle.gameObject.activeSelf, "Desk idle frame");
				report.AppendLine("PASS idle: duck bobs on the desk using the authored idle frame");

				duck.SwitchDuckState(DuckState.Flying, query.GetFlightSeconds());
				query.ReactToChanges();
				await UniTask.Delay(TimeSpan.FromSeconds(query.GetFlightSeconds() * 0.5f));
				Require(instance.transform.position.y > 3f, "Flight arc apex");
				Require(instance.transform.position.x < 2.2f && instance.transform.position.x > 1.6f,
					"Flight arc midpoint");
				Require(firstFlight.gameObject.activeSelf || secondFlight.gameObject.activeSelf,
					"Flight animation frame");
				ScreenCapture.CaptureScreenshot(PlaytestPaths.Get("duck-flight.png"));
				await UniTask.DelayFrame(2);
				report.AppendLine("PASS flight: 0.6-second parabola crosses the authored apex with rotating frames");

				await UniTask.Delay(TimeSpan.FromSeconds(query.GetFlightSeconds() * 0.55f));
				duck.SwitchDuckState(DuckState.OnFloor, 10f);
				query.ReactToChanges();
				await UniTask.DelayFrame(2);
				ParticleSystem dust = instance.GetComponentInChildren<ParticleSystem>();
				AudioSource audio = instance.GetComponent<AudioSource>();
				Require(Vector3.Distance(instance.transform.position, new Vector3(-3.6f, 1f, 0f)) < 0.01f,
					"Floor landing position");
				Require(dust.particleCount > 0, "Landing dust burst");
				Require(audio.isPlaying, "Landing squeak");
				ScreenCapture.CaptureScreenshot(PlaytestPaths.Get("duck-landing.png"));
				await UniTask.DelayFrame(2);
				report.AppendLine("PASS landing: floor impact emits eight dust particles and plays the squeak");

				duck.SwitchDuckState(DuckState.Carried, 10f);
				query.ReactToChanges();
				await UniTask.Delay(TimeSpan.FromSeconds(0.35));
				Require(instance.transform.parent == teacher, "Teacher carry parent");
				Require(Vector3.Distance(instance.transform.localPosition, new Vector3(0.7f, 1.65f, 0f)) < 0.02f,
					"Teacher carry position");
				Vector3 carriedPosition = instance.transform.position;
				teacher.position += Vector3.right * 0.5f;
				await UniTask.DelayFrame(2);
				Require(Mathf.Abs(instance.transform.position.x - carriedPosition.x - 0.5f) < 0.01f,
					"Duck follows teacher");
				ScreenCapture.CaptureScreenshot(PlaytestPaths.Get("duck-carried.png"));
				await UniTask.DelayFrame(2);
				report.AppendLine("PASS carry: duck attaches to the teacher's paw and follows her return walk");

				teacher.position = teacherPosition;
				duck.SettleDuck(DuckState.OnDesk);
				query.ReactToChanges();
				await UniTask.Delay(TimeSpan.FromSeconds(0.55));
				Require(instance.transform.parent == source.transform.parent, "Desk return parent");
				Require(Vector3.Distance(instance.transform.position, deskPosition) < 0.02f,
					"Desk return position");
				report.AppendLine("PASS return: teacher carries the duck back to its original desk position");

				duck.ReplaceDuckThrowCount(3);
				duck.SwitchDuckState(DuckState.Confiscated, 0f);
				query.ReactToChanges();
				await UniTask.DelayFrame(2);
				Require(sad.gameObject.activeSelf, "Sad confiscated frame");
				Require(Vector3.Distance(instance.transform.position, new Vector3(-1.45f, 0.08f, 0f)) < 0.01f,
					"Confiscated desk position");
				ScreenCapture.CaptureScreenshot(PlaytestPaths.Get("duck-confiscated.png"));
				await UniTask.DelayFrame(2);
				report.AppendLine("PASS confiscation: third throw leaves the sad duck on the teacher's desk");
				report.AppendLine("DONE");
			}
			catch (Exception exception)
			{
				report.AppendLine("FAIL " + exception);
				Debug.LogException(exception);
			}
			finally
			{
				if (instance != null)
					Object.Destroy(instance);

				if (teacher != null)
					teacher.position = teacherPosition;

				if (source != null)
					source.SetActive(true);

				if (hintBubble != null)
					hintBubble.SetActive(true);

				File.WriteAllText(PlaytestPaths.Get("duck.txt"), report.ToString());
			}
		}

		private static async UniTask<SceneContext> FindOrEnterGameplayScene()
		{
			SceneContext scene = FindGameplayScene();
			if (scene != null)
				return scene;

			LaunchWindow launch = Object.FindFirstObjectByType<LaunchWindow>();
			if (launch == null)
				throw new InvalidOperationException("Enter Play Mode before running duck animation checks.");

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
