using System;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Microphone
{
	public class MicrophoneService : IMicrophoneService, IInitializable, IDisposable
	{
		private readonly float[] _samples = new float[SampleWindow];

		private string _deviceName;
		private AudioClip _clip;

		private const int SampleWindow = 1024;
		private const int ClipLengthSeconds = 1;
		private const int PreferredFrequency = 44100;

		public bool IsAvailable => _clip != null && UnityEngine.Microphone.IsRecording(_deviceName);

		public void Initialize()
		{
			StartRecording();
		}

		public void Dispose()
		{
			if (_clip == null)
				return;

			UnityEngine.Microphone.End(_deviceName);

			_deviceName = null;
			_clip = null;
		}

		public float GetRootMeanSquare()
		{
			if (IsAvailable == false)
				return 0f;

			_clip.GetData(_samples, GetSampleOffset());

			float squareSum = 0f;

			foreach (float sample in _samples)
			{
				squareSum += sample * sample;
			}

			return Mathf.Sqrt(squareSum / SampleWindow);
		}

		private void StartRecording()
		{
			if (UnityEngine.Microphone.devices.Length == 0)
				return;

			_deviceName = UnityEngine.Microphone.devices[0];

			try
			{
				_clip = UnityEngine.Microphone.Start(
					deviceName: _deviceName,
					loop: true,
					lengthSec: ClipLengthSeconds,
					frequency: GetFrequency());
			}
			catch (Exception exception)
			{
				Debug.LogWarning($"Microphone '{_deviceName}' is unavailable: {exception.Message}");

				_deviceName = null;
				_clip = null;
			}
		}

		private int GetFrequency()
		{
			UnityEngine.Microphone.GetDeviceCaps(_deviceName, out int minimumFrequency, out int maximumFrequency);

			if (maximumFrequency == 0)
				return PreferredFrequency;

			return Mathf.Clamp(PreferredFrequency, minimumFrequency, maximumFrequency);
		}

		private int GetSampleOffset()
		{
			int position = UnityEngine.Microphone.GetPosition(_deviceName);

			if (position >= SampleWindow)
				return position - SampleWindow;

			return _clip.samples - SampleWindow;
		}
	}
}
