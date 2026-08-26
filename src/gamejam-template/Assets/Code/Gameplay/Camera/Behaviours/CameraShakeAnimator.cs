using Code.Gameplay.Camera.Configs;
using Code.Infrastructure.EntityComponentSystem;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Camera.Behaviours
{
	public class CameraShakeAnimator : EntityComponentProvider
	{
		[SF] private Transform shakeTarget;

		private Vector3 _initialLocalPosition;
		private Quaternion _initialLocalRotation;

		private ShakeConfig _config;
		private float _elapsed;
		private float _scale;
		private bool _isPlaying;

		private float _positionSeedX;
		private float _positionSeedY;
		private float _positionSeedZ;
		private float _rotationSeedX;
		private float _rotationSeedY;
		private float _rotationSeedZ;

		public override void RegisterComponents() => Entity.AddCameraShakeAnimator(this);
		public override void UnregisterComponents() => Entity.SafeRemoveCameraShakeAnimator();

		protected override void OnAwake()
		{
			if (shakeTarget == null)
			{
				Debug.LogError($"{nameof(CameraShakeAnimator)} on {name} requires a shake target.");
				shakeTarget = transform;
			}

			_initialLocalPosition = shakeTarget.localPosition;
			_initialLocalRotation = shakeTarget.localRotation;
		}

		private void OnDisable()
		{
			Stop();
		}

		private void LateUpdate()
		{
			if (_isPlaying == false)
				return;

			_elapsed += Time.deltaTime;

			float progress = Mathf.Clamp01(_elapsed / _config.Duration);
			float intensity = _config.IntensityCurve.Evaluate(progress) * _scale;
			float time = _elapsed * _config.Frequency;

			Vector3 positionOffset = Vector3.Scale(
				new Vector3(
					Noise(_positionSeedX, time),
					Noise(_positionSeedY, time),
					Noise(_positionSeedZ, time)),
				_config.PositionAmplitude) * intensity;

			Vector3 rotationOffset = Vector3.Scale(
				new Vector3(
					Noise(_rotationSeedX, time),
					Noise(_rotationSeedY, time),
					Noise(_rotationSeedZ, time)),
				_config.RotationAmplitude) * intensity;

			shakeTarget.localPosition = _initialLocalPosition + positionOffset;
			shakeTarget.localRotation = _initialLocalRotation * Quaternion.Euler(rotationOffset);

			if (_elapsed >= _config.Duration)
				Stop();
		}

		public void Play(ShakeConfig config, float scale = 1f)
		{
			if (config == null)
			{
				Debug.LogError($"{nameof(CameraShakeAnimator)} on {name} received a null shake config.");
				Stop();
				return;
			}

			if (config.Duration <= 0f)
			{
				Debug.LogError($"{nameof(CameraShakeAnimator)} on {name} received {nameof(ShakeConfig)} with non-positive duration.");
				Stop();
				return;
			}

			Stop();

			_config = config;
			_elapsed = 0f;
			_scale = Mathf.Max(0f, scale);
			_isPlaying = true;

			_positionSeedX = RandomSeed();
			_positionSeedY = RandomSeed();
			_positionSeedZ = RandomSeed();
			_rotationSeedX = RandomSeed();
			_rotationSeedY = RandomSeed();
			_rotationSeedZ = RandomSeed();
		}

		private void Stop()
		{
			_isPlaying = false;
			_elapsed = 0f;
			_config = null;

			if (shakeTarget == null)
				return;

			shakeTarget.localPosition = _initialLocalPosition;
			shakeTarget.localRotation = _initialLocalRotation;
		}

		private static float RandomSeed() => Random.value * 1000f;

		private static float Noise(float seed, float time)
		{
			return Mathf.PerlinNoise(seed, time) * 2f - 1f;
		}
	}
}
