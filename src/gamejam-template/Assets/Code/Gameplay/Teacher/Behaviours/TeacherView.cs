using System.Collections.Generic;
using Code.Gameplay.Teacher.Queries;
using DG.Tweening;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Teacher.Behaviours
{
	public class TeacherView : MonoBehaviour
	{
		[SF] private Transform[] poses;

		private Vector3[] _poseOrigins;
		private Vector3[] _poseRotations;
		private Vector3[] _poseScales;
		private Transform[] _heads;
		private Transform[] _arms;
		private Vector3[] _armRotations;
		private Transform[][] _pupils;
		private Vector3[][] _pupilOrigins;
		private Vector3 _origin;
		private TeacherAttention _attention;

		private Tween _pose;
		private Tween _gaze;
		private Tween _chalk;
		private Tween _walk;

		private ITeacherQuery _query;

		private const string HeadName = "Head";
		private const string ArmName = "teacher_arm_chalk";
		private const string PupilName = "teacher_pupil";

		private const float ChalkDegrees = 8f;
		private const float ChalkStrokeSeconds = 0.3f;
		private const float TurnHeadDegrees = 16f;
		private const float TurnHeadSeconds = 0.15f;
		private const float TurnPoseScale = 0.96f;
		private const float TurnPoseSeconds = 0.3f;
		private const float GazeOffsetUnits = 0.06f;
		private const float GazeSeconds = 0.12f;
		private const float GazePauseMinimumSeconds = 0.3f;
		private const float GazePauseMaximumSeconds = 0.6f;
		private const float StareSeconds = 0.25f;
		private const float AlertDegrees = 14f;
		private const float AlertScale = 0.08f;
		private const float AlertSeconds = 0.3f;
		private const float WalkOffsetUnits = -4.5f;
		private const float WalkSeconds = 1f;
		private const float StepUnits = 0.06f;
		private const float StepSeconds = 0.25f;

		private void Awake()
		{
			_origin = transform.localPosition;
			_poseOrigins = new Vector3[poses.Length];
			_poseRotations = new Vector3[poses.Length];
			_poseScales = new Vector3[poses.Length];
			_heads = new Transform[poses.Length];
			_arms = new Transform[poses.Length];
			_armRotations = new Vector3[poses.Length];
			_pupils = new Transform[poses.Length][];
			_pupilOrigins = new Vector3[poses.Length][];

			for (int index = 0; index < poses.Length; index++)
				CachePose(index);
		}

		private void OnDestroy() => Unbind();

		public void Bind(ITeacherQuery query)
		{
			Unbind();
			_query = query;
			_query.OnAttentionChanged += HandleAttention;
			HandleAttention(_query.GetAttention());
		}

		public void Unbind()
		{
			if (_query != null)
				_query.OnAttentionChanged -= HandleAttention;

			_query = null;
			StopTweens();
		}

		private void CachePose(int index)
		{
			Transform pose = poses[index];
			_poseOrigins[index] = pose.localPosition;
			_poseRotations[index] = pose.localEulerAngles;
			_poseScales[index] = pose.localScale;
			_heads[index] = pose.Find(HeadName);
			_arms[index] = pose.Find(ArmName);
			_armRotations[index] = _arms[index] == null ? Vector3.zero : _arms[index].localEulerAngles;

			List<Transform> pupils = new(2);
			foreach (Transform child in _heads[index])
			{
				if (child.name == PupilName)
					pupils.Add(child);
			}

			_pupils[index] = pupils.ToArray();
			_pupilOrigins[index] = new Vector3[pupils.Count];
			for (int pupil = 0; pupil < pupils.Count; pupil++)
				_pupilOrigins[index][pupil] = pupils[pupil].localPosition;
		}

		private void StopTweens()
		{
			_pose = Stop(_pose);
			_gaze = Stop(_gaze);
			_chalk = Stop(_chalk);
			_walk = Stop(_walk);
		}

		private void ResetPose(TeacherAttention attention)
		{
			int index = (int)attention;
			Transform pose = poses[index];
			pose.localPosition = _poseOrigins[index];
			pose.localEulerAngles = _poseRotations[index];
			pose.localScale = _poseScales[index];
			_heads[index].localEulerAngles = Vector3.zero;

			if (_arms[index] != null)
				_arms[index].localEulerAngles = _armRotations[index];

			for (int pupil = 0; pupil < _pupils[index].Length; pupil++)
				_pupils[index][pupil].localPosition = _pupilOrigins[index][pupil];
		}

		private void ShowPose(TeacherAttention attention)
		{
			for (int index = 0; index < poses.Length; index++)
				poses[index].gameObject.SetActive(index == (int)attention);
		}

		private void PlayWriting()
		{
			int index = (int)TeacherAttention.Writing;
			Transform arm = _arms[index];
			arm.localEulerAngles = _armRotations[index] + new Vector3(0f, 0f, -ChalkDegrees);
			_chalk = arm
				.DOLocalRotate(_armRotations[index] + new Vector3(0f, 0f, ChalkDegrees), ChalkStrokeSeconds)
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo);
		}

		private void PlayTurning()
		{
			int index = (int)TeacherAttention.Turning;
			Transform pose = poses[index];
			_heads[index].localEulerAngles = new Vector3(0f, 0f, -TurnHeadDegrees);
			pose.localScale = _poseScales[index] * TurnPoseScale;
			_pose = DOTween.Sequence()
				.Join(_heads[index].DOLocalRotate(Vector3.zero, TurnHeadSeconds).SetEase(Ease.OutBack))
				.Join(pose.DOScale(_poseScales[index], TurnPoseSeconds).SetEase(Ease.OutBack));
		}

		private void PlayStaring()
		{
			int index = (int)TeacherAttention.Staring;
			Transform pose = poses[index];
			pose.localEulerAngles = Vector3.zero;
			pose.localScale = Vector3.one;
			_pose = DOTween.Sequence()
				.Join(pose.DOLocalRotate(_poseRotations[index], StareSeconds).SetEase(Ease.OutBack))
				.Join(pose.DOScale(_poseScales[index], StareSeconds).SetEase(Ease.OutBack));
		}

		private void PlayAlerted()
		{
			Transform pose = poses[(int)TeacherAttention.Alerted];
			_pose = DOTween.Sequence()
				.Join(pose.DOPunchRotation(new Vector3(0f, 0f, AlertDegrees), AlertSeconds, 8, 0.6f))
				.Join(pose.DOPunchScale(Vector3.one * AlertScale, AlertSeconds, 8, 0.6f));
		}

		private void PlayDistracted()
		{
			int index = (int)TeacherAttention.Distracted;
			_pose = poses[index]
				.DOLocalMoveY(_poseOrigins[index].y + StepUnits, StepSeconds)
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo);
		}

		private void PlayGaze()
		{
			int index = (int)_attention;
			Transform[] pupils = _pupils[index];
			if (pupils.Length == 0)
				return;

			float offset = Random.Range(-GazeOffsetUnits, GazeOffsetUnits);
			Sequence gaze = DOTween.Sequence();
			for (int pupil = 0; pupil < pupils.Length; pupil++)
			{
				gaze.Join(pupils[pupil]
					.DOLocalMoveX(_pupilOrigins[index][pupil].x + offset, GazeSeconds)
					.SetEase(Ease.OutQuad));
			}

			gaze.AppendInterval(Random.Range(GazePauseMinimumSeconds, GazePauseMaximumSeconds));
			gaze.OnComplete(PlayGaze);
			_gaze = gaze;
		}

		private void WalkTo(float x)
		{
			if (Mathf.Approximately(transform.localPosition.x, x))
				return;

			_walk = transform.DOLocalMoveX(x, WalkSeconds).SetEase(Ease.InOutSine);
		}

		private static Tween Stop(Tween tween)
		{
			if (tween != null)
				tween.Kill();

			return null;
		}

		private void HandleAttention(TeacherAttention attention)
		{
			StopTweens();
			ResetPose(_attention);
			_attention = attention;
			ShowPose(attention);
			WalkTo(attention == TeacherAttention.Distracted ? _origin.x + WalkOffsetUnits : _origin.x);

			switch (attention)
			{
				case TeacherAttention.Writing: PlayWriting(); break;
				case TeacherAttention.Turning: PlayTurning(); break;
				case TeacherAttention.Watching: PlayGaze(); break;
				case TeacherAttention.Staring: PlayStaring(); break;
				case TeacherAttention.Alerted: PlayAlerted(); break;
				case TeacherAttention.Distracted: PlayDistracted(); break;
			}
		}
	}
}
