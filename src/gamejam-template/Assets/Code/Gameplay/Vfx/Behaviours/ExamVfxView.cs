using Code.Gameplay.Duck;
using Code.Gameplay.Duck.Queries;
using Code.Gameplay.Exam.Data;
using Code.Gameplay.Exam.Queries;
using Code.Gameplay.Input.Queries;
using Code.Gameplay.Neighbours;
using Code.Gameplay.Teacher;
using Code.Gameplay.Teacher.Queries;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Vfx.Behaviours
{
	public class ExamVfxView : MonoBehaviour
	{
		[SF] private ParticleSystem sweat;
		[SF] private ParticleSystem alert;
		[SF] private ParticleSystem laughterLeft;
		[SF] private ParticleSystem laughterRight;
		[SF] private ParticleSystem chalk;
		[SF] private ParticleSystem sparkles;
		[SF] private float sweatLeanOffset = 0.6f;

		private ParticleSystem[] _emitters;
		private ParticleSystem[] _bursts;
		private Vector3 _sweatOrigin;
		private bool _leaning;
		private TeacherAttention _attention;

		private IExamQuery _exam;
		private ITeacherQuery _teacher;
		private IInputQuery _input;
		private IDuckQuery _duck;

		private const int AlertCount = 1;
		private const int LaughterCount = 6;
		private const int SparkleCount = 10;

		private void Awake()
		{
			_emitters = new[] { sweat, alert, laughterLeft, laughterRight, chalk, sparkles };
			_bursts = new[] { alert, laughterLeft, laughterRight, sparkles };
			_sweatOrigin = sweat.transform.localPosition;
		}

		private void OnDestroy() => Unbind();

		public void Bind(ExamVfxDto queries)
		{
			Unbind();
			_exam = queries.Exam;
			_teacher = queries.Teacher;
			_input = queries.Input;
			_duck = queries.Duck;
			_exam.OnAnswerCopied += HandleAnswerCopied;
			_teacher.OnAttentionChanged += HandleAttention;
			_input.OnLeanChanged += HandleLean;
			_duck.OnStateChanged += HandleDuckState;

			_leaning = _input.IsLeaning();
			_attention = _teacher.GetAttention();

			foreach (ParticleSystem emitter in _bursts)
				emitter.Play(true);

			RefreshLoops();
		}

		public void Unbind()
		{
			if (_exam != null)
				_exam.OnAnswerCopied -= HandleAnswerCopied;

			if (_teacher != null)
				_teacher.OnAttentionChanged -= HandleAttention;

			if (_input != null)
				_input.OnLeanChanged -= HandleLean;

			if (_duck != null)
				_duck.OnStateChanged -= HandleDuckState;

			_exam = null;
			_teacher = null;
			_input = null;
			_duck = null;
			StopAll();
		}

		public void Configure(ExamVfxEmittersDto emitters)
		{
			sweat = emitters.Sweat;
			alert = emitters.Alert;
			laughterLeft = emitters.LaughterLeft;
			laughterRight = emitters.LaughterRight;
			chalk = emitters.Chalk;
			sparkles = emitters.Sparkles;
		}

		private void StopAll()
		{
			if (_emitters == null)
				return;

			foreach (ParticleSystem emitter in _emitters)
				emitter.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		}

		private void RefreshLoops()
		{
			SetLoopPlaying(chalk, _attention == TeacherAttention.Writing);
			SetLoopPlaying(sweat, _leaning && _attention == TeacherAttention.Watching);
		}

		private void SetLoopPlaying(ParticleSystem emitter, bool playing)
		{
			if (playing == emitter.isEmitting)
				return;

			if (playing == false)
			{
				emitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				return;
			}

			if (emitter == sweat)
				MoveSweatToLeanSide();

			emitter.Play(true);
		}

		private void MoveSweatToLeanSide()
		{
			QuestionDefinition question = _exam.GetCurrentQuestion();
			float direction = question != null && question.Neighbour == NeighbourSide.Right ? 1f : -1f;
			sweat.transform.localPosition = _sweatOrigin + Vector3.right * (sweatLeanOffset * direction);
		}

		private void HandleAnswerCopied(int questionIndex) => sparkles.Emit(SparkleCount);

		private void HandleLean(bool leaning)
		{
			_leaning = leaning;
			RefreshLoops();
		}

		private void HandleAttention(TeacherAttention attention)
		{
			_attention = attention;
			RefreshLoops();

			if (attention == TeacherAttention.Turning || attention == TeacherAttention.Alerted)
				alert.Emit(AlertCount);
		}

		private void HandleDuckState(DuckState state)
		{
			if (state != DuckState.Flying)
				return;

			laughterLeft.Emit(LaughterCount);
			laughterRight.Emit(LaughterCount);
		}
	}
}
