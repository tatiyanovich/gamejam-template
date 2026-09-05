using Code.Gameplay.Exam.Data;
using Code.Gameplay.Exam.Queries;
using Code.Gameplay.Input.Queries;
using Code.Gameplay.Neighbours;
using Code.Gameplay.Teacher;
using Code.Gameplay.Teacher.Queries;
using DG.Tweening;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Input.Behaviours
{
	public class KittenView : MonoBehaviour
	{
		[SF] private Transform[] poses;

		private Vector3[] _posePositions;
		private Vector3[] _poseRotations;
		private Vector3[] _poseScales;
		private Transform[] _tails;
		private Vector3[] _tailRotations;
		private Transform[] _typingPaws;
		private Vector3[] _typingPawRotations;
		private int _poseIndex;
		private int _questionIndex;
		private int _answerProgress;
		private bool _leaning;
		private TeacherAttention _attention;

		private Tween _pose;
		private Tween _tail;
		private Tween _typing;

		private IInputQuery _inputQuery;
		private IExamQuery _examQuery;
		private ITeacherQuery _teacherQuery;

		private const int IdlePoseIndex = 0;
		private const int LeanLeftPoseIndex = 1;
		private const int LeanRightPoseIndex = 2;
		private const int PanicLeftPoseIndex = 3;
		private const int PanicRightPoseIndex = 4;

		private const float BreathScale = 1.02f;
		private const float IdleHalfCycleSeconds = 0.8f;
		private const float TailDegrees = 10f;
		private const float LeanSeconds = 0.2f;
		private const float TypingDegrees = 15f;
		private const float TypingHalfSeconds = 0.06f;

		private void Awake()
		{
			_posePositions = new Vector3[poses.Length];
			_poseRotations = new Vector3[poses.Length];
			_poseScales = new Vector3[poses.Length];
			_tails = new Transform[poses.Length];
			_tailRotations = new Vector3[poses.Length];
			_typingPaws = new Transform[poses.Length];
			_typingPawRotations = new Vector3[poses.Length];

			for (int index = 0; index < poses.Length; index++)
				CachePose(index);

			_poseIndex = IdlePoseIndex;
		}

		private void OnDestroy() => Unbind();

		public void Bind(IInputQuery inputQuery, IExamQuery examQuery, ITeacherQuery teacherQuery)
		{
			Unbind();
			_inputQuery = inputQuery;
			_examQuery = examQuery;
			_teacherQuery = teacherQuery;
			_inputQuery.OnLeanChanged += HandleLean;
			_examQuery.OnCurrentQuestionChanged += HandleCurrentQuestion;
			_examQuery.OnAnswerProgressChanged += HandleAnswerProgress;
			_teacherQuery.OnAttentionChanged += HandleAttention;

			_questionIndex = _examQuery.GetCurrentQuestionIndex();
			_answerProgress = _examQuery.GetAnswerProgress();
			_leaning = _inputQuery.IsLeaning();
			_attention = _teacherQuery.GetAttention();
			RefreshPose();
		}

		public void Unbind()
		{
			if (_inputQuery != null)
				_inputQuery.OnLeanChanged -= HandleLean;

			if (_examQuery != null)
			{
				_examQuery.OnCurrentQuestionChanged -= HandleCurrentQuestion;
				_examQuery.OnAnswerProgressChanged -= HandleAnswerProgress;
			}

			if (_teacherQuery != null)
				_teacherQuery.OnAttentionChanged -= HandleAttention;

			_inputQuery = null;
			_examQuery = null;
			_teacherQuery = null;
			StopTweens();
			ResetPoses();
			ShowPose(IdlePoseIndex);
			_poseIndex = IdlePoseIndex;
		}

		public void Configure(Transform[] values) => poses = values;

		private void CachePose(int index)
		{
			Transform pose = poses[index];
			_posePositions[index] = pose.localPosition;
			_poseRotations[index] = pose.localEulerAngles;
			_poseScales[index] = pose.localScale;
			_tails[index] = pose.Find("tail");
			_tailRotations[index] = _tails[index].localEulerAngles;
			_typingPaws[index] = pose.Find("pawRight");
			_typingPawRotations[index] = _typingPaws[index].localEulerAngles;
		}

		private void StopTweens()
		{
			_pose = Stop(_pose);
			_tail = Stop(_tail);
			_typing = Stop(_typing);
		}

		private void ResetPoses()
		{
			for (int index = 0; index < poses.Length; index++)
				ResetPose(index);
		}

		private void ResetPose(int index)
		{
			poses[index].localPosition = _posePositions[index];
			poses[index].localEulerAngles = _poseRotations[index];
			poses[index].localScale = _poseScales[index];
			_tails[index].localEulerAngles = _tailRotations[index];
			_typingPaws[index].localEulerAngles = _typingPawRotations[index];
		}

		private void ShowPose(int index)
		{
			for (int pose = 0; pose < poses.Length; pose++)
				poses[pose].gameObject.SetActive(pose == index);
		}

		private void RefreshPose()
		{
			int nextPoseIndex = GetPoseIndex();
			StopTweens();
			ResetPose(_poseIndex);
			ResetPose(nextPoseIndex);
			ShowPose(nextPoseIndex);
			_poseIndex = nextPoseIndex;

			if (_leaning)
				PlayLean();
			else
				PlayIdle();
		}

		private int GetPoseIndex()
		{
			if (_leaning == false)
				return IdlePoseIndex;

			bool left = GetNeighbourSide() != NeighbourSide.Right;
			bool panicking = _attention == TeacherAttention.Watching;
			if (panicking)
				return left ? PanicLeftPoseIndex : PanicRightPoseIndex;

			return left ? LeanLeftPoseIndex : LeanRightPoseIndex;
		}

		private NeighbourSide GetNeighbourSide()
		{
			QuestionDefinition question = _examQuery.GetCurrentQuestion();
			return question == null ? NeighbourSide.Left : question.Neighbour;
		}

		private void PlayIdle()
		{
			Transform idle = poses[IdlePoseIndex];
			Transform tail = _tails[IdlePoseIndex];
			tail.localEulerAngles = _tailRotations[IdlePoseIndex] + new Vector3(0f, 0f, -TailDegrees);
			_pose = idle
				.DOScale(_poseScales[IdlePoseIndex] * BreathScale, IdleHalfCycleSeconds)
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo);
			_tail = tail
				.DOLocalRotate(_tailRotations[IdlePoseIndex] + new Vector3(0f, 0f, TailDegrees),
					IdleHalfCycleSeconds)
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo);
		}

		private void PlayLean()
		{
			Transform pose = poses[_poseIndex];
			Vector3 targetPosition = _posePositions[_poseIndex];
			Vector3 targetRotation = _poseRotations[_poseIndex];
			pose.localPosition = new Vector3(0f, targetPosition.y, targetPosition.z);
			pose.localEulerAngles = Vector3.zero;
			_pose = DOTween.Sequence()
				.Join(pose.DOLocalMove(targetPosition, LeanSeconds).SetEase(Ease.OutCubic))
				.Join(pose.DOLocalRotate(targetRotation, LeanSeconds).SetEase(Ease.OutCubic));
		}

		private void PlayTyping()
		{
			_typing = Stop(_typing);
			Transform paw = _typingPaws[_poseIndex];
			Vector3 rotation = _typingPawRotations[_poseIndex];
			paw.localEulerAngles = rotation;
			_typing = DOTween.Sequence()
				.Append(paw.DOLocalRotate(rotation + new Vector3(0f, 0f, -TypingDegrees), TypingHalfSeconds)
					.SetEase(Ease.OutQuad))
				.Append(paw.DOLocalRotate(rotation, TypingHalfSeconds).SetEase(Ease.InQuad));
		}

		private static Tween Stop(Tween tween)
		{
			if (tween != null)
				tween.Kill();

			return null;
		}

		private void HandleLean(bool leaning)
		{
			_leaning = leaning;
			RefreshPose();
		}

		private void HandleCurrentQuestion(int questionIndex)
		{
			_questionIndex = questionIndex;
			_answerProgress = _examQuery.GetAnswerProgress();
			if (_leaning)
				RefreshPose();
		}

		private void HandleAnswerProgress(int questionIndex, int progress, int length)
		{
			if (questionIndex != _questionIndex)
				return;

			bool advanced = progress > _answerProgress;
			_answerProgress = progress;
			if (advanced && _leaning)
				PlayTyping();
		}

		private void HandleAttention(TeacherAttention attention)
		{
			bool wasPanicking = _attention == TeacherAttention.Watching;
			_attention = attention;
			bool isPanicking = _attention == TeacherAttention.Watching;
			if (_leaning && wasPanicking != isPanicking)
				RefreshPose();
		}
	}
}
