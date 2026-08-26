using System;
using System.Collections.Generic;
using Framework.Essentials.ScriptableObjects;
using UnityEngine;

namespace Framework.Essentials.TimeManagement
{
	public class UnityTimeService : ITimeService
	{
		private readonly List<PauseRequestHandler> _requests = new();

		private float _slowdown = NoSlowdown;
		private float _scaleBeforeOverride = NoSlowdown;
		private bool _overriding;
		private bool _paused;

		private const float NoSlowdown = 1f;
		private const float MinSlowdown = 0.01f;

		public float DeltaTime => IsPaused() == false ? UnityEngine.Time.deltaTime : 0;
		
		public float UnscaledDeltaTime => IsPaused() == false ? UnityEngine.Time.unscaledDeltaTime : 0;
		public float FixedDeltaTime => UnityEngine.Time.fixedDeltaTime;
		public float Time => UnityEngine.Time.time;
		public DateTime UtcNow => DateTime.UtcNow;
		public float Slowdown => _slowdown;

		public event Action OnPause;
		public event Action OnResume;

		public PauseRequestHandler RequestPause()
		{
			PauseRequestHandler handler = new(IDUtility.GenerateID());
			_requests.Add(handler);
			RefreshTimeScale();

			return handler;
		}

		public void ReleasePause(PauseRequestHandler handler)
		{
			if (_requests.Contains(handler) == false)
				return;

			_requests.Remove(handler);
			RefreshTimeScale();
		}

		public bool IsPaused() => _requests.Count > 0;
		public void SetSlowdown(float factor)
		{
			factor = Mathf.Clamp(factor, MinSlowdown, NoSlowdown);

			if (Mathf.Approximately(factor, _slowdown))
				return;

			_slowdown = factor;
			RefreshTimeScale();
		}

		private void RefreshTimeScale()
		{
			bool paused = IsPaused();
			bool overriding = paused || _slowdown < NoSlowdown;

			if (overriding)
			{
				if (_overriding == false)
				{
					_scaleBeforeOverride = UnityEngine.Time.timeScale;
					_overriding = true;
				}

				UnityEngine.Time.timeScale = paused ? 0f : _scaleBeforeOverride * _slowdown;
			}
			else if (_overriding)
			{
				_overriding = false;
				UnityEngine.Time.timeScale = _scaleBeforeOverride;
			}

			RefreshPauseState(paused);
		}

		private void RefreshPauseState(bool paused)
		{
			if (paused == _paused)
				return;

			_paused = paused;

			if (paused)
				OnPause?.Invoke();
			else
				OnResume?.Invoke();
		}
	}
}
