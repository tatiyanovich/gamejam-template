using System;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Microphone
{
	public class MicrophoneService : IMicrophoneService, IInitializable, IDisposable
	{
		private readonly float[] _samples = new float[SampleWindow];

		private string _defaultDeviceName;
		private AudioClip _clip;
		private int _lastPosition;
		private float _lastProgressTime;
		private bool _initialized;

		private const int SampleWindow = 1024;
		private const int ClipLengthSeconds = 1;
		private const int PreferredFrequency = 44100;
		private const int LowSampleRateMaximumFrequency = 24000;
		private const float LowSampleRateLevelMultiplier = 0.5f;

		public bool IsAvailable => _clip != null
			&& UnityEngine.Microphone.IsRecording(null)
			&& UnityEngine.Microphone.GetPosition(null) > 0;

		public void Initialize()
		{
			if (_initialized == false)
				AudioSettings.OnAudioConfigurationChanged += HandleAudioConfigurationChanged;

			_initialized = true;
			StartRecording();
		}

		public void Dispose()
		{
			AudioSettings.OnAudioConfigurationChanged -= HandleAudioConfigurationChanged;
			_initialized = false;
			StopRecording();
		}

		public float GetRootMeanSquare()
		{
			RefreshRecording();

			if (IsAvailable == false)
				return 0f;

			_clip.GetData(_samples, GetSampleOffset());

			float squareSum = 0f;

			foreach (float sample in _samples)
			{
				squareSum += sample * sample;
			}

			float rootMeanSquare = Mathf.Sqrt(squareSum / SampleWindow);
			return rootMeanSquare * GetInputLevelMultiplier();
		}

		private void StartRecording()
		{
			StopRecording();

			if (UnityEngine.Microphone.devices.Length == 0)
				return;

			_defaultDeviceName = UnityEngine.Microphone.devices[0];

			try
			{
				_clip = UnityEngine.Microphone.Start(
					deviceName: null,
					loop: true,
					lengthSec: ClipLengthSeconds,
					frequency: GetFrequency());
				_lastPosition = 0;
				_lastProgressTime = Time.realtimeSinceStartup;
			}
			catch (Exception exception)
			{
				Debug.LogWarning($"Default microphone is unavailable: {exception.Message}");

				_defaultDeviceName = null;
				_clip = null;
			}
		}

		private void StopRecording()
		{
			if (_clip != null)
				UnityEngine.Microphone.End(null);

			_defaultDeviceName = null;
			_clip = null;
			_lastPosition = 0;
		}

		private void RefreshRecording()
		{
			string defaultDevice = UnityEngine.Microphone.devices.Length > 0
				? UnityEngine.Microphone.devices[0]
				: null;
			if (_clip == null || _defaultDeviceName != defaultDevice
				|| UnityEngine.Microphone.IsRecording(null) == false)
			{
				StartRecording();
				return;
			}

			int position = UnityEngine.Microphone.GetPosition(null);
			if (position != _lastPosition)
			{
				_lastPosition = position;
				_lastProgressTime = Time.realtimeSinceStartup;
				return;
			}

			if (Time.realtimeSinceStartup - _lastProgressTime >= ClipLengthSeconds)
				StartRecording();
		}

		private int GetFrequency()
		{
			UnityEngine.Microphone.GetDeviceCaps(null, out int minimumFrequency, out int maximumFrequency);

			if (maximumFrequency == 0)
				return PreferredFrequency;

			return Mathf.Clamp(PreferredFrequency, minimumFrequency, maximumFrequency);
		}

		private int GetSampleOffset()
		{
			int position = UnityEngine.Microphone.GetPosition(null);

			if (position >= SampleWindow)
				return position - SampleWindow;

			return _clip.samples - SampleWindow;
		}

		private float GetInputLevelMultiplier() => _clip.frequency <= LowSampleRateMaximumFrequency
			? LowSampleRateLevelMultiplier
			: 1f;

		private void HandleAudioConfigurationChanged(bool deviceWasChanged)
		{
			if (deviceWasChanged)
				StartRecording();
		}
	}
}
