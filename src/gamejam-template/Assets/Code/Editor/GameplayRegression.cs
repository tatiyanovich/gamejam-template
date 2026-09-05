using System;
using System.IO;
using Code.Gameplay.Exam;
using Code.Gameplay.Difficulty.Services;
using Code.Gameplay.Duck;
using Code.Gameplay.Duck.Services;
using Code.Gameplay.Meow.Systems;
using Code.Gameplay.Neighbours;
using Code.Gameplay.Neighbours.Services;
using Code.Gameplay.Teacher;
using Code.Gameplay.Teacher.Services;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Code.Editor
{
	public static class GameplayRegression
	{
		private static string ReportPath => PlaytestPaths.Get("regression.txt");

		[MenuItem("COPYCAT/QA/Test gameplay boundaries")]
		public static void Run()
		{
			File.WriteAllText(ReportPath, "H4 isolated production pipeline tests\n");
			SceneContext scene = null;
			foreach (SceneContext candidate in UnityEngine.Object.FindObjectsByType<SceneContext>(FindObjectsSortMode.None))
			{
				if (candidate.gameObject.scene.name == "Gameplay")
					scene = candidate;
			}
			if (scene == null)
			{
				File.AppendAllText(ReportPath, "NOT RUN: enter the exam before running gameplay regression.\n");
				Debug.LogWarning("Enter the exam before running gameplay regression.");
				return;
			}
			Check(scene.Container, "duck plus meow is safe", DuckMeow);
			Check(scene.Container, "staring retains meow extension", StaringMeow);
			Check(scene.Container, "keyboard meow disarms microphone", KeyboardMeow);
			Check(scene.Container, "last accepted answer beats bell", LastAnswer);
			Check(scene.Container, "accepted answer counted at bell", AnswerAtBell);
			Check(scene.Container, "finished exam ignores meow", FinishedMeow);
			Check(scene.Container, "stroke gate, mistake and pause", StrokeGate);
			Check(scene.Container, "pick mistake and correct option", PickAnswer);
			Check(scene.Container, "word mistake retains progress and alerts teacher", WordAnswer);
			Check(scene.Container, "paw refresh and expiration", PawWindow);
			Check(scene.Container, "three throws, cooldown and confiscation", DuckCycle);
			Check(scene.Container, "phase one safe and later checks", TeacherChecks);
			Check(scene.Container, "suspicion gain, decay and caught", Suspicion);
			Check(scene.Container, "bell announcement and freeze", Bell);
			File.AppendAllText(ReportPath, "DONE\n");
		}

		private static void Check(DiContainer container, string name, Action<GameplayPlaytestFixture> test)
		{
			try
			{
				using GameplayPlaytestFixture fixture = new(container);
				test(fixture);
				File.AppendAllText(ReportPath, $"PASS {name}\n");
			}
			catch (Exception exception)
			{
				File.AppendAllText(ReportPath, $"FAIL {name}: {exception}\n");
			}
		}

		private static GameEntity Teacher(GameplayPlaytestFixture fixture, TeacherAttention attention)
		{
			GameEntity teacher = fixture.Container.Resolve<ITeacherFactory>().CreateTeacher();
			teacher.SwitchAttention(attention, 0.5f);
			teacher.isTeacherFacingClass = attention.IsFacingClass();
			return teacher;
		}

		private static void Meow(GameplayPlaytestFixture fixture)
		{
			GameEntity meow = fixture.Game.CreateEntity();
			meow.isEvent = true;
			meow.isReady = true;
			meow.AddMeowEvent(false);
		}

		private static void DuckMeow(GameplayPlaytestFixture fixture)
		{
			fixture.Run.ReplaceCurrentQuestionIndex(5);
			fixture.Run.ReplaceSuspicionLevel(50f);
			fixture.Keyboard.isLeanHeld = true;
			GameEntity teacher = Teacher(fixture, TeacherAttention.Watching);
			Meow(fixture);
			GameEntity duck = fixture.Game.CreateEntity();
			duck.isEvent = true;
			duck.isReady = true;
			duck.isDuckThrownEvent = true;
			fixture.Core.Execute();
			Assert(teacher.TeacherAttention == TeacherAttention.Distracted && teacher.isTeacherFacingClass == false,
				"Distracted teacher must stop facing class immediately");
			Assert(Mathf.Approximately(fixture.Run.SuspicionLevel, 30f), $"Suspicion={fixture.Run.SuspicionLevel}");
		}

		private static void StaringMeow(GameplayPlaytestFixture fixture)
		{
			fixture.Run.ReplaceCurrentQuestionIndex(5);
			fixture.Keyboard.isLeanHeld = true;
			GameEntity teacher = Teacher(fixture, TeacherAttention.Staring);
			Meow(fixture);
			fixture.Core.Execute();
			Assert(Mathf.Approximately(teacher.TeacherAttentionTimeLeft, 1.5f),
				$"Stare duration={teacher.TeacherAttentionTimeLeft}");
			AssertRemark(fixture, TeacherRemark.MeowWhileWatching);
		}

		private static void AssertRemark(GameplayPlaytestFixture fixture, TeacherRemark expected)
		{
			GameEntity remark = fixture.Game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.TeacherRemarkEvent)).GetSingleEntity();
			Assert(remark.TeacherRemarkEvent == expected, $"Teacher remark={remark.TeacherRemarkEvent}");
		}

		private static void KeyboardMeow(GameplayPlaytestFixture fixture)
		{
			GameEntity source = fixture.Game.CreateEntity();
			source.isMeowSource = true;
			source.isMeowArmed = true;
			source.AddMicrophoneLevel(0f);
			fixture.Keyboard.isMeowKeyPressed = true;
			fixture.Container.Instantiate<EmitMeowOnKeyPressedSystem>().Execute();
			Assert(source.isMeowArmed == false, "Mic stays armed during keyboard cooldown");
		}

		private static void LastAnswer(GameplayPlaytestFixture fixture)
		{
			AnswerBeforeBell(fixture, 11);
			Assert(fixture.Run.AnswersCopied == 12 && fixture.Run.ExamOutcome == ExamOutcome.Passed,
				$"Answers={fixture.Run.AnswersCopied}, outcome={fixture.Run.ExamOutcome}");
		}

		private static void AnswerAtBell(GameplayPlaytestFixture fixture)
		{
			AnswerBeforeBell(fixture, 0);
			Assert(fixture.Run.AnswersCopied == 1 && fixture.Run.ExamOutcome == ExamOutcome.BellRang,
				$"Answers={fixture.Run.AnswersCopied}, outcome={fixture.Run.ExamOutcome}");
		}

		private static void AnswerBeforeBell(GameplayPlaytestFixture fixture, int index)
		{
			fixture.Run.ReplaceCurrentQuestionIndex(index);
			fixture.Run.ReplaceAnswersCopied(index);
			fixture.Run.ReplaceExamElapsedSeconds(119.99f);
			fixture.Time.DeltaTime = 0.02f;
			GameEntity question = fixture.Exams.CreateQuestion(index);
			question.ReplaceAnswerProgress(question.AnswerLength - 1);
			GameEntity neighbour = fixture.Container.Resolve<INeighbourFactory>().CreateNeighbour(question.AnswerNeighbourSide);
			neighbour.isPawLifted = true;
			neighbour.ReplacePawWindowTimeLeft(2f);
			fixture.Keyboard.isLeanHeld = true;
			fixture.Keyboard.ReplaceStrokeInput(question.AnswerStrokes[question.AnswerLength - 1]);
			fixture.Core.Execute();
		}

		private static void FinishedMeow(GameplayPlaytestFixture fixture)
		{
			fixture.Run.isExamFinished = true;
			fixture.Exams.CreateQuestion(0);
			GameEntity neighbour = fixture.Container.Resolve<INeighbourFactory>().CreateNeighbour(NeighbourSide.Left);
			GameEntity teacher = Teacher(fixture, TeacherAttention.Watching);
			Meow(fixture);
			fixture.Core.Execute();
			Assert(neighbour.isPawLifted == false && Mathf.Approximately(teacher.TeacherAttentionTimeLeft, 0.5f),
				"Meow changes a finished attempt");
		}

		private static GameEntity OpenQuestion(GameplayPlaytestFixture fixture, int index)
		{
			fixture.Run.ReplaceCurrentQuestionIndex(index);
			GameEntity question = fixture.Exams.CreateQuestion(index);
			GameEntity neighbour = fixture.Container.Resolve<INeighbourFactory>().CreateNeighbour(question.AnswerNeighbourSide);
			neighbour.isPawLifted = true;
			neighbour.ReplacePawWindowTimeLeft(10f);
			fixture.Keyboard.isLeanHeld = true;
			return question;
		}

		private static void StrokeGate(GameplayPlaytestFixture fixture)
		{
			GameEntity question = OpenQuestion(fixture, 0);
			fixture.Keyboard.isLeanHeld = false;
			fixture.Keyboard.ReplaceStrokeInput(StrokeDirection.Up);
			fixture.Tick(0f);
			Assert(question.AnswerProgress == 0, "Upright input accepted");
			fixture.Keyboard.isLeanHeld = true;
			fixture.Tick(0f);
			Assert(question.AnswerProgress == 1, "First stroke rejected");
			fixture.Tick(0f);
			fixture.Keyboard.SafeRemoveStrokeInput();
			fixture.Tick(0f);
			Assert(question.AnswerProgress == 1 && fixture.Run.SuspicionLevel == 8f, "Wrong stroke penalty");
			fixture.Keyboard.ReplaceStrokeInput(StrokeDirection.Right);
			fixture.Tick(0f);
			fixture.Keyboard.SafeRemoveStrokeInput();
			fixture.Tick(0f);
			Assert(question.isAnswerCopied && fixture.Run.AnswersCopied == 1, "Answer counted more than once");
			Assert(question.LifetimeLeft > 0.5f, "Copied pause missing");
		}

		private static void PickAnswer(GameplayPlaytestFixture fixture)
		{
			GameEntity question = OpenQuestion(fixture, 3);
			fixture.Keyboard.ReplacePickInput(1);
			fixture.Tick(0f);
			fixture.Keyboard.SafeRemovePickInput();
			fixture.Tick(0f);
			Assert(question.AnswerProgress == 0 && fixture.Run.SuspicionLevel == 8f, "Wrong pick penalty");
			fixture.Keyboard.ReplacePickInput(0);
			fixture.Tick(0f);
			Assert(question.isAnswerCopied && fixture.Run.AnswersCopied == 1, "Correct pick rejected");
		}

		private static void WordAnswer(GameplayPlaytestFixture fixture)
		{
			GameEntity question = OpenQuestion(fixture, 5);
			GameEntity teacher = fixture.Container.Resolve<ITeacherFactory>().CreateTeacher();
			fixture.Keyboard.ReplaceLetterInput('B');
			fixture.Tick(0f);
			fixture.Keyboard.ReplaceLetterInput('Z');
			fixture.Tick(0f);
			fixture.Keyboard.SafeRemoveLetterInput();
			fixture.Tick(0f);
			Assert(question.AnswerProgress == 1 && fixture.Run.SuspicionLevel == 8f, "Wrong word penalty");
			Assert(teacher.TeacherAttention == TeacherAttention.Alerted, "Pencil snap did not alert phase three");
			AssertRemark(fixture, TeacherRemark.PencilAlert);
			foreach (char letter in "IRD")
			{
				fixture.Keyboard.ReplaceLetterInput(letter);
				fixture.Tick(0f);
			}
			Assert(question.isAnswerCopied, "Word did not finish");
		}

		private static void PawWindow(GameplayPlaytestFixture fixture)
		{
			GameEntity question = OpenQuestion(fixture, 0);
			fixture.Keyboard.ReplaceStrokeInput(StrokeDirection.Up);
			fixture.Tick(0f);
			fixture.Keyboard.SafeRemoveStrokeInput();
			Meow(fixture);
			fixture.Tick(1f);
			GameEntity neighbour = fixture.Game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Neighbour)).GetSingleEntity();
			Assert(Mathf.Approximately(neighbour.PawWindowTimeLeft, 9f), "Paw window stacks");
			fixture.Tick(10f);
			fixture.Keyboard.ReplaceStrokeInput(StrokeDirection.Right);
			fixture.Tick(0f);
			Assert(question.AnswerProgress == 1 && question.isAnswerReadable == false, "Covered paw accepts input");
			Meow(fixture);
			fixture.Tick(0f);
			Assert(question.isAnswerCopied, "Paw reopening lost progress");
		}

		private static void DuckCycle(GameplayPlaytestFixture fixture)
		{
			IDuckFactory factory = fixture.Container.Resolve<IDuckFactory>();
			GameEntity duck = factory.CreateDuck();
			Teacher(fixture, TeacherAttention.Watching);
			for (int index = 1; index <= 3; index++)
			{
				fixture.Run.ReplaceSuspicionLevel(50f);
				factory.CreateThrowDuckRequest();
				factory.CreateThrowDuckRequest();
				fixture.Tick(0f);
				Assert(duck.DuckThrowCount == index && duck.DuckState == DuckState.Flying, "Duplicate throw");
				fixture.Tick(0f);
				Assert(fixture.Run.SuspicionLevel == 30f, "Duck reduction is not twenty");
				factory.CreateThrowDuckRequest();
				fixture.Tick(0.6f);
				Assert(duck.DuckThrowCount == index && duck.DuckState == DuckState.OnFloor, "Flying throw accepted");
				fixture.Tick(3.4f);
				if (index < 3)
				{
					Assert(duck.DuckState == DuckState.Carried, "Duck was not picked up");
					fixture.Tick(8f);
					Assert(duck.DuckState == DuckState.OnDesk, "Duck did not return");
				}
			}
			factory.CreateThrowDuckRequest();
			fixture.Tick(0f);
			Assert(duck.DuckState == DuckState.Confiscated && duck.DuckThrowCount == 3, "Fourth throw accepted");
		}

		private static void TeacherChecks(GameplayPlaytestFixture fixture)
		{
			GameEntity teacher = fixture.Container.Resolve<ITeacherFactory>().CreateTeacher();
			fixture.Tick(10f);
			Assert(teacher.TeacherAttention == TeacherAttention.Writing
				&& teacher.hasTeacherAttentionTimeLeft == false, "Phase one has checks");
			fixture.Run.ReplaceCurrentQuestionIndex(5);
			fixture.Tick(0f);
			fixture.Tick(teacher.TeacherAttentionTimeLeft);
			Assert(teacher.TeacherAttention == TeacherAttention.Turning, "Telegraph skipped");
			fixture.Keyboard.isLeanHeld = true;
			fixture.Tick(0.3f);
			Assert(teacher.TeacherAttention == TeacherAttention.Watching && teacher.AlmostCaughtCount == 1,
				"Watching or almost caught missing");
			fixture.Run.ReplaceSuspicionLevel(0f);
			fixture.Tick(teacher.TeacherAttentionTimeLeft);
			Assert(teacher.TeacherAttention == TeacherAttention.Staring, "Leaning did not retain stare");
			fixture.Run.isExamFinished = false;
			fixture.Run.ReplaceSuspicionLevel(0f);
			fixture.Keyboard.isLeanHeld = false;
			fixture.Tick(0.5f);
			Assert(teacher.TeacherAttention == TeacherAttention.Writing, "Stare release is not half a second");
			float[] windows = { 10f, 10f, 10f, 7f, 7f, 5f, 5f, 5f, 4f, 4f, 3f, 3f };
			for (int index = 0; index < windows.Length; index++)
				Assert(fixture.Container.Resolve<IDifficultyService>().GetPhase(index).PawWindow == windows[index],
					$"Wrong phase for question {index + 1}");
		}

		private static void Suspicion(GameplayPlaytestFixture fixture)
		{
			GameEntity teacher = Teacher(fixture, TeacherAttention.Watching);
			teacher.ReplaceTeacherAttentionTimeLeft(10f);
			fixture.Keyboard.isLeanHeld = true;
			fixture.Tick(1f);
			Assert(fixture.Run.SuspicionLevel == 35f, "Watching rate is not 35 per second");
			fixture.Keyboard.isLeanHeld = false;
			fixture.Tick(1f);
			Assert(fixture.Run.SuspicionLevel == 35f, "Suspicion decays under watch");
			teacher.ReturnToWriting();
			fixture.Tick(1f);
			Assert(fixture.Run.SuspicionLevel == 30f, "Decay is not 5 per second");
			teacher.SwitchAttention(TeacherAttention.Watching, 10f);
			fixture.Keyboard.isLeanHeld = true;
			fixture.Tick(2f);
			Assert(fixture.Run.ExamOutcome == ExamOutcome.Caught && fixture.Run.SuspicionLevel == 100f,
				"Maximum suspicion did not finish attempt");
		}

		private static void Bell(GameplayPlaytestFixture fixture)
		{
			fixture.Run.ReplaceExamElapsedSeconds(74.9f);
			fixture.Tick(0.1f);
			Assert(fixture.Run.isBellAnnounced, "45 second announcement missing");
			fixture.Tick(45f);
			Assert(fixture.Run.ExamOutcome == ExamOutcome.BellRang, "Bell did not finish attempt");
			float elapsed = fixture.Run.ExamElapsedSeconds;
			fixture.Tick(1f);
			Assert(fixture.Run.ExamElapsedSeconds == elapsed, "Clock continued after finish");
		}

		private static void Assert(bool passed, string message)
		{
			if (passed == false)
				throw new InvalidOperationException(message);
		}
	}
}
