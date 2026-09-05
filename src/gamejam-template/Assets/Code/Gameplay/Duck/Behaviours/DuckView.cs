using Code.Gameplay.Duck.Queries;
using DG.Tweening;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Duck.Behaviours
{
	public class DuckView : MonoBehaviour
	{
		[SF] private Transform[] frames;
		[SF] private ParticleSystem landingDust;
		[SF] private AudioSource audioSource;

		private Transform _sceneParent;
		private Transform _teacher;
		private Vector3 _deskPosition;
		private Quaternion _deskRotation;
		private Vector3 _deskScale;
		private Vector3 _flightControl;
		private DuckState _state;
		private AudioClip _squeak;

		private Tween _motion;
		private Tween _frameSwap;

		private IDuckQuery _query;

		private static readonly Vector3 FlightStart = new(6.9f, -3.15f, 0f);
		private static readonly Vector3 FlightApex = new(1.9f, 3.3f, 0f);
		private static readonly Vector3 FloorPosition = new(-3.6f, 1f, 0f);
		private static readonly Vector3 ConfiscatedPosition = new(-1.45f, 0.08f, 0f);
		private static readonly Vector3 CarryPosition = new(0.7f, 1.65f, 0f);

		private const int IdleFrameIndex = 0;
		private const int FirstFlightFrameIndex = 1;
		private const int SecondFlightFrameIndex = 2;
		private const int SadFrameIndex = 3;

		private const float DeskBobUnits = 0.06f;
		private const float DeskBobHalfCycleSeconds = 0.6f;
		private const float FlightRotationDegrees = 720f;
		private const float FlightFrameSeconds = 0.1f;
		private const float FlightApexScale = 0.7f;
		private const float FloorScale = 0.42f;
		private const float CarryScale = 0.55f;
		private const float ConfiscatedScale = 0.55f;
		private const float PickUpSeconds = 0.3f;
		private const float ReturnSeconds = 0.5f;
		private const float SqueakSeconds = 0.18f;
		private const float SqueakVolume = 0.35f;
		private const int AudioSampleRate = 44100;

		private void Awake()
		{
			_sceneParent = transform.parent;
			_deskPosition = transform.position;
			_deskRotation = transform.rotation;
			_deskScale = transform.localScale;
			_squeak = CreateSqueak();
		}

		private void OnDestroy()
		{
			Unbind();
			if (_squeak != null)
				Destroy(_squeak);
		}

		public void Bind(IDuckQuery query, Transform teacher)
		{
			Unbind();
			_query = query;
			_teacher = teacher;
			_query.OnStateChanged += HandleState;
			SetState(_query.GetState(), false);
		}

		public void Unbind()
		{
			if (_query != null)
				_query.OnStateChanged -= HandleState;

			_query = null;
			_teacher = null;
			StopTweens();
			SetParent(_sceneParent, true);
		}

		public void Configure(Transform[] values, ParticleSystem dust, AudioSource source)
		{
			frames = values;
			landingDust = dust;
			audioSource = source;
		}

		private void StopTweens()
		{
			_motion = Stop(_motion);
			_frameSwap = Stop(_frameSwap);
		}

		private void SetState(DuckState state, bool animate)
		{
			DuckState previous = _state;
			_state = state;
			StopTweens();

			switch (state)
			{
				case DuckState.OnDesk: ShowOnDesk(animate && previous == DuckState.Carried); break;
				case DuckState.Flying: PlayFlight(); break;
				case DuckState.OnFloor: ShowOnFloor(animate); break;
				case DuckState.Carried: PlayPickUp(); break;
				case DuckState.Confiscated: ShowConfiscated(); break;
			}
		}

		private void ShowOnDesk(bool animateReturn)
		{
			SetParent(_sceneParent, true);
			ShowFrame(IdleFrameIndex);
			transform.rotation = _deskRotation;

			if (animateReturn)
			{
				_motion = DOTween.Sequence()
					.Join(transform.DOMove(_deskPosition, ReturnSeconds).SetEase(Ease.InOutSine))
					.Join(transform.DOScale(_deskScale, ReturnSeconds).SetEase(Ease.InOutSine))
					.OnComplete(PlayDeskBob);
				return;
			}

			transform.position = _deskPosition;
			transform.localScale = _deskScale;
			PlayDeskBob();
		}

		private void PlayDeskBob()
		{
			Vector3 origin = transform.localPosition;
			Vector3 offset = Vector3.up * DeskBobUnits;
			_motion = DOTween.Sequence()
				.Append(transform.DOLocalMove(origin + offset, DeskBobHalfCycleSeconds * 0.5f)
					.SetEase(Ease.OutSine))
				.Append(transform.DOLocalMove(origin - offset, DeskBobHalfCycleSeconds)
					.SetEase(Ease.InOutSine))
				.Append(transform.DOLocalMove(origin, DeskBobHalfCycleSeconds * 0.5f)
					.SetEase(Ease.InSine))
				.SetLoops(-1);
		}

		private void PlayFlight()
		{
			SetParent(_sceneParent, true);
			ShowFrame(FirstFlightFrameIndex);
			transform.position = FlightStart;
			transform.rotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			_flightControl = FlightApex * 2f - (FlightStart + FloorPosition) * 0.5f;
			float flightSeconds = _query.GetFlightSeconds();

			_motion = DOTween.Sequence()
				.Join(DOTween.To(() => 0f, SetFlightProgress, 1f, flightSeconds).SetEase(Ease.Linear))
				.Join(transform.DORotate(Vector3.forward * -FlightRotationDegrees, flightSeconds,
					RotateMode.FastBeyond360).SetEase(Ease.Linear))
				.Join(DOTween.Sequence()
					.Append(transform.DOScale(Vector3.one * FlightApexScale, flightSeconds * 0.5f)
						.SetEase(Ease.OutSine))
					.Append(transform.DOScale(Vector3.one * FloorScale, flightSeconds * 0.5f)
						.SetEase(Ease.InSine)));

			_frameSwap = DOTween.Sequence()
				.AppendInterval(FlightFrameSeconds)
				.AppendCallback(() => ShowFrame(SecondFlightFrameIndex))
				.AppendInterval(FlightFrameSeconds)
				.AppendCallback(() => ShowFrame(FirstFlightFrameIndex))
				.SetLoops(-1);
		}

		private void SetFlightProgress(float progress)
		{
			float remaining = 1f - progress;
			transform.position = remaining * remaining * FlightStart
				+ 2f * remaining * progress * _flightControl
				+ progress * progress * FloorPosition;
		}

		private void ShowOnFloor(bool playLanding)
		{
			SetParent(_sceneParent, true);
			ShowFrame(IdleFrameIndex);
			transform.position = FloorPosition;
			transform.rotation = Quaternion.identity;
			transform.localScale = Vector3.one * FloorScale;

			if (playLanding == false)
				return;

			landingDust.Play(
				withChildren: true);
			audioSource.clip = _squeak;
			audioSource.Play();
			_motion = transform.DOPunchScale(Vector3.one * 0.08f, 0.2f, 5, 0.5f);
		}

		private void PlayPickUp()
		{
			SetParent(_teacher, true);
			ShowFrame(IdleFrameIndex);
			transform.rotation = Quaternion.identity;
			Vector3 targetScale = Vector3.one * CarryScale / Mathf.Abs(_teacher.lossyScale.x);
			_motion = DOTween.Sequence()
				.Join(transform.DOLocalMove(CarryPosition, PickUpSeconds).SetEase(Ease.OutBack))
				.Join(transform.DOScale(targetScale, PickUpSeconds).SetEase(Ease.OutBack));
		}

		private void ShowConfiscated()
		{
			SetParent(_sceneParent, true);
			ShowFrame(SadFrameIndex);
			transform.position = ConfiscatedPosition;
			transform.rotation = Quaternion.identity;
			transform.localScale = Vector3.one * ConfiscatedScale;
		}

		private void ShowFrame(int activeIndex)
		{
			for (int index = 0; index < frames.Length; index++)
				frames[index].gameObject.SetActive(index == activeIndex);
		}

		private void SetParent(Transform parent, bool worldPositionStays)
		{
			if (transform.parent != parent)
				transform.SetParent(parent, worldPositionStays);
		}

		private static Tween Stop(Tween tween)
		{
			if (tween != null)
				tween.Kill();

			return null;
		}

		private static AudioClip CreateSqueak()
		{
			int sampleCount = Mathf.CeilToInt(AudioSampleRate * SqueakSeconds);
			float[] samples = new float[sampleCount];
			float phase = 0f;
			for (int sample = 0; sample < sampleCount; sample++)
			{
				float progress = (float)sample / sampleCount;
				float arc = 1f - Mathf.Abs(progress * 2f - 1f);
				float frequency = Mathf.Lerp(650f, 1400f, arc);
				phase += 2f * Mathf.PI * frequency / AudioSampleRate;
				float envelope = Mathf.Sin(Mathf.PI * progress) * (1f - progress);
				samples[sample] = (Mathf.Sin(phase) + Mathf.Sin(phase * 2f) * 0.25f)
					* envelope * SqueakVolume;
			}

			AudioClip clip = AudioClip.Create("DuckSqueak", sampleCount, 1, AudioSampleRate, false);
			clip.SetData(samples, 0);
			return clip;
		}

		private void HandleState(DuckState state) => SetState(state, true);
	}
}
