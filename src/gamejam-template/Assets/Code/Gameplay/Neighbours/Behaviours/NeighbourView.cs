using Code.Gameplay.Neighbours.Queries;
using Code.Gameplay.Teacher;
using Code.Gameplay.Teacher.Queries;
using DG.Tweening;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Neighbours.Behaviours
{
	public class NeighbourView : MonoBehaviour
	{
		[SF] private Transform paw;
		[SF] private Transform head;

		private Vector3 _pawOriginPosition;
		private Vector3 _pawOriginRotation;
		private Vector3 _headOriginPosition;
		private Vector3 _headOriginRotation;
		private NeighbourSide _side;
		private bool _pawLifted;
		private bool _teacherFacingClass;
		private bool _telegraphing;
		private float _pawWindowTimeLeft;

		private Tween _pawTween;
		private Tween _telegraphTween;
		private Tween _headTween;

		private INeighbourQuery _neighbourQuery;
		private ITeacherQuery _teacherQuery;

		private const float LiftDegrees = 70f;
		private const float LiftUnits = 0.4f;
		private const float LiftSeconds = 0.25f;
		private const float LiftOvershoot = 1.70158f;
		private const float CoverSeconds = 0.35f;
		private const float TelegraphSeconds = 1f;
		private const float TelegraphDegrees = 3f;
		private const float TelegraphHalfCycleSeconds = 0.08f;
		private const float RestartEpsilonSeconds = 0.05f;
		private const float HeadJoltUnits = 0.14f;
		private const float HeadJoltDegrees = 5f;
		private const float HeadJoltSeconds = 0.22f;
		private const float HeadDuckUnits = 0.18f;
		private const float HeadDuckSeconds = 0.12f;

		private Vector3 PawLiftedPosition => _pawOriginPosition + Vector3.up * LiftUnits;
		private Vector3 PawLiftedRotation => _pawOriginRotation + Vector3.forward * LiftDegrees;
		private Vector3 HeadTargetPosition => _headOriginPosition
			+ Vector3.down * (_teacherFacingClass ? HeadDuckUnits : 0f);

		private void Awake()
		{
			_pawOriginPosition = paw.localPosition;
			_pawOriginRotation = paw.localEulerAngles;
			_headOriginPosition = head.localPosition;
			_headOriginRotation = head.localEulerAngles;
		}

		private void OnDestroy() => Unbind();

		public void Bind(INeighbourQuery neighbourQuery, ITeacherQuery teacherQuery)
		{
			Unbind();
			_neighbourQuery = neighbourQuery;
			_teacherQuery = teacherQuery;
			_side = transform.position.x < 0f ? NeighbourSide.Left : NeighbourSide.Right;
			_neighbourQuery.OnPawChanged += HandlePaw;
			_teacherQuery.OnAttentionChanged += HandleAttention;

			_pawLifted = _neighbourQuery.IsPawLifted(_side);
			_pawWindowTimeLeft = _neighbourQuery.GetPawWindowTimeLeft(_side);
			_teacherFacingClass = _teacherQuery.IsFacingClass();
			SetPaw(_pawLifted);
			SetHead();
			RefreshTelegraph();
		}

		public void Unbind()
		{
			if (_neighbourQuery != null)
				_neighbourQuery.OnPawChanged -= HandlePaw;

			if (_teacherQuery != null)
				_teacherQuery.OnAttentionChanged -= HandleAttention;

			_neighbourQuery = null;
			_teacherQuery = null;
			StopTweens();
			_pawLifted = false;
			_teacherFacingClass = false;
			_telegraphing = false;
			_pawWindowTimeLeft = 0f;
			SetPaw(false);
			SetHead();
		}

		public void Configure(Transform pawTransform, Transform headTransform)
		{
			paw = pawTransform;
			head = headTransform;
		}

		private void StopTweens()
		{
			_pawTween = Stop(_pawTween);
			_telegraphTween = Stop(_telegraphTween);
			_headTween = Stop(_headTween);
		}

		private void SetPaw(bool lifted)
		{
			paw.localPosition = lifted ? PawLiftedPosition : _pawOriginPosition;
			paw.localEulerAngles = lifted ? PawLiftedRotation : _pawOriginRotation;
		}

		private void SetHead()
		{
			head.localPosition = HeadTargetPosition;
			head.localEulerAngles = _headOriginRotation;
		}

		private void PlayLift()
		{
			_pawTween = Stop(_pawTween);
			_telegraphTween = Stop(_telegraphTween);
			_telegraphing = false;
			_pawTween = DOTween.Sequence()
				.Join(paw.DOLocalMove(PawLiftedPosition, LiftSeconds).SetEase(Ease.OutBack, LiftOvershoot))
				.Join(paw.DOLocalRotate(PawLiftedRotation, LiftSeconds).SetEase(Ease.OutBack, LiftOvershoot));
		}

		private void PlayCover()
		{
			_pawTween = Stop(_pawTween);
			_telegraphTween = Stop(_telegraphTween);
			_telegraphing = false;
			_pawTween = DOTween.Sequence()
				.Join(paw.DOLocalMove(_pawOriginPosition, CoverSeconds).SetEase(Ease.InOutSine))
				.Join(paw.DOLocalRotate(_pawOriginRotation, CoverSeconds).SetEase(Ease.InOutSine));
		}

		private void PlayTelegraph()
		{
			_pawTween = Stop(_pawTween);
			_telegraphTween = Stop(_telegraphTween);
			_telegraphing = true;
			paw.localPosition = PawLiftedPosition;
			paw.localEulerAngles = PawLiftedRotation - Vector3.forward * TelegraphDegrees;
			_telegraphTween = paw
				.DOLocalRotate(PawLiftedRotation + Vector3.forward * TelegraphDegrees,
					TelegraphHalfCycleSeconds)
				.SetEase(Ease.Linear)
				.SetLoops(-1, LoopType.Yoyo);
		}

		private void PlayHeadJolt()
		{
			_headTween = Stop(_headTween);
			SetHead();
			_headTween = DOTween.Sequence()
				.Join(head.DOPunchPosition(Vector3.up * HeadJoltUnits, HeadJoltSeconds, 7, 0.5f))
				.Join(head.DOPunchRotation(Vector3.forward * HeadJoltDegrees, HeadJoltSeconds, 7, 0.5f));
		}

		private void PlayHeadFocus()
		{
			_headTween = Stop(_headTween);
			head.localEulerAngles = _headOriginRotation;
			_headTween = head.DOLocalMove(HeadTargetPosition, HeadDuckSeconds).SetEase(Ease.OutQuad);
		}

		private void RefreshTelegraph()
		{
			if (_pawLifted && _pawWindowTimeLeft > 0f && _pawWindowTimeLeft <= TelegraphSeconds)
			{
				if (_telegraphing == false)
					PlayTelegraph();

				return;
			}

			if (_telegraphing)
			{
				_telegraphTween = Stop(_telegraphTween);
				_telegraphing = false;
				SetPaw(_pawLifted);
			}
		}

		private static Tween Stop(Tween tween)
		{
			if (tween != null)
				tween.Kill();

			return null;
		}

		private void HandlePaw(NeighbourSide side, bool lifted, float seconds)
		{
			if (side != _side)
				return;

			bool liftedNow = lifted && _pawLifted == false;
			bool coveredNow = lifted == false && _pawLifted;
			bool restarted = lifted && _pawLifted && seconds > _pawWindowTimeLeft + RestartEpsilonSeconds;
			_pawLifted = lifted;
			_pawWindowTimeLeft = seconds;

			if (liftedNow)
				PlayLift();
			else if (coveredNow)
				PlayCover();
			else if (restarted)
			{
				_telegraphTween = Stop(_telegraphTween);
				_telegraphing = false;
				SetPaw(true);
			}

			if (liftedNow || restarted)
				PlayHeadJolt();

			RefreshTelegraph();
		}

		private void HandleAttention(TeacherAttention attention)
		{
			bool facingClass = attention.IsFacingClass();
			if (_teacherFacingClass == facingClass)
				return;

			_teacherFacingClass = facingClass;
			PlayHeadFocus();
		}
	}
}
