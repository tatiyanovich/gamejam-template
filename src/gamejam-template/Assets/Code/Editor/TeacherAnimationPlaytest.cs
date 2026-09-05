using System;
using System.IO;
using System.Text;
using Code.Gameplay.Teacher;
using Code.Gameplay.Teacher.Behaviours;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Code.Editor
{
	public static class TeacherAnimationPlaytest
	{
		private const float HoldSeconds = 10f;
		private const float SlowMotionScale = 0.1f;

		[MenuItem("COPYCAT/QA/Test teacher animation")]
		public static void Run() => RunChecks().Forget();

		private static async UniTask RunChecks()
		{
			StringBuilder report = new();
			TeacherView view = Object.FindFirstObjectByType<TeacherView>(FindObjectsInactive.Include);
			float timeScale = Time.timeScale;
			try
			{
				if (EditorApplication.isPlaying == false || view == null)
					throw new InvalidOperationException("Enter the exam before running teacher animation checks.");

				Time.timeScale = SlowMotionScale;
				GameContext game = ProjectContext.Instance.Container.Resolve<GameContext>();
				GameEntity teacher = game.GetGroup(GameMatcher
					.AllOf(
						GameMatcher.Teacher)).GetSingleEntity();
				Transform root = view.transform;
				float origin = root.localPosition.x;

				await Switch(teacher, TeacherAttention.Writing);
				Transform arm = Pose(view, TeacherAttention.Writing).Find("teacher_arm_chalk");
				float firstStroke = arm.localEulerAngles.z;
				await UniTask.Delay(TimeSpan.FromSeconds(0.2));
				Require(Mathf.Abs(Mathf.DeltaAngle(firstStroke, arm.localEulerAngles.z)) > 1f, "Chalk stroke");
				Require(Active(view, TeacherAttention.Writing), "Writing pose");
				report.AppendLine("PASS writing: back pose visible, chalk arm swings");

				await Switch(teacher, TeacherAttention.Turning);
				Transform head = Pose(view, TeacherAttention.Turning).Find("Head");
				Require(Active(view, TeacherAttention.Turning), "Turning pose");
				float telegraph = 0f;
				for (int sample = 0; sample < 30 && telegraph <= 5f; sample++)
				{
					telegraph = Mathf.Abs(Mathf.DeltaAngle(head.localEulerAngles.z, 0f));
					if (telegraph <= 5f)
						await UniTask.NextFrame();
				}
				Require(telegraph > 5f, $"Turning telegraph, head off by {telegraph:0.0} degrees");
				await UniTask.Delay(TimeSpan.FromSeconds(0.35));
				Require(Mathf.Abs(Mathf.DeltaAngle(head.localEulerAngles.z, 0f)) < 1f, "Turning settles");
				report.AppendLine("PASS turning: head snaps around within the 0.3-second telegraph");

				await Switch(teacher, TeacherAttention.Watching);
				Transform pupil = Pose(view, TeacherAttention.Watching).Find("Head").Find("teacher_pupil");
				float firstGaze = pupil.localPosition.x;
				bool moved = false;
				for (int sample = 0; sample < 12 && moved == false; sample++)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(0.15));
					moved = Mathf.Abs(pupil.localPosition.x - firstGaze) > 0.005f;
				}
				Require(Active(view, TeacherAttention.Watching), "Watching pose");
				Require(moved, "Gaze darts");
				report.AppendLine("PASS watching: front pose visible, pupils dart across the rows");

				await Switch(teacher, TeacherAttention.Staring);
				Transform stare = Pose(view, TeacherAttention.Staring);
				Require(Active(view, TeacherAttention.Staring), "Staring pose");
				Require(Mathf.Abs(Mathf.DeltaAngle(stare.localEulerAngles.z, 0f)) < 3f, "Staring starts upright");
				await UniTask.Delay(TimeSpan.FromSeconds(0.4));
				Require(Mathf.Abs(Mathf.DeltaAngle(stare.localEulerAngles.z, -6f)) < 1f, "Staring lean");
				Require(Mathf.Abs(stare.localScale.x - 1.05f) < 0.01f, "Staring scale");
				report.AppendLine("PASS staring: leans in to the authored 6 degrees and 1.05 scale");

				await Switch(teacher, TeacherAttention.Alerted);
				Transform alerted = Pose(view, TeacherAttention.Alerted);
				bool punched = false;
				for (int sample = 0; sample < 8 && punched == false; sample++)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(0.04));
					punched = Mathf.Abs(Mathf.DeltaAngle(alerted.localEulerAngles.z, 0f)) > 1f;
				}
				Require(Active(view, TeacherAttention.Alerted), "Alerted pose");
				Require(punched, "Alerted snap");
				report.AppendLine("PASS alerted: sharp turn punch on the front pose");

				await Switch(teacher, TeacherAttention.Distracted);
				await UniTask.Delay(TimeSpan.FromSeconds(1.2));
				Require(Active(view, TeacherAttention.Distracted), "Distracted pose");
				Require(root.localPosition.x < origin - 2.5f, "Walks to the duck");
				report.AppendLine("PASS distracted: back pose walks left towards the blackboard");

				await Switch(teacher, TeacherAttention.Writing);
				await UniTask.Delay(TimeSpan.FromSeconds(1.2));
				Require(Mathf.Abs(root.localPosition.x - origin) < 0.01f, "Walks back");
				report.AppendLine("PASS return: she walks back to the blackboard spot and writes again");
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
				File.WriteAllText(PlaytestPaths.Get("teacher.txt"), report.ToString());
			}
		}

		private static async UniTask Switch(GameEntity teacher, TeacherAttention attention)
		{
			teacher.SwitchAttention(attention, HoldSeconds);
			await UniTask.DelayFrame(2);
		}

		private static Transform Pose(TeacherView view, TeacherAttention attention)
		{
			return view.transform.Find(attention.ToString().ToLowerInvariant());
		}

		private static bool Active(TeacherView view, TeacherAttention attention)
		{
			return Pose(view, attention).gameObject.activeSelf;
		}

		private static void Require(bool condition, string message)
		{
			if (condition == false)
				throw new InvalidOperationException(message);
		}
	}
}
