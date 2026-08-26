using UnityEditor;
using UnityEngine;

namespace Code.Editor.Profiling
{
	public sealed class PerfHudWindow : EditorWindow
	{
		private const float CountInterval = 1f;
		private const float WarmupDuration = 2f;

		private bool _countObjects = true;

		private int _batches;
		private int _setPassCalls;
		private int _peakBatches;
		private int _peakSetPassCalls;

		private float _fps;
		private float _minFps;
		private float _worstFrameMs;
		private float _fpsSum;
		private int _fpsSamples;

		private int _spriteRenderers;
		private int _activeSpriteRenderers;
		private int _transforms;

		private double _nextCountTime;
		private double _measureStartTime;

		[MenuItem("Tools/Perf HUD")]
		private static void Open() => GetWindow<PerfHudWindow>("Perf HUD");

		private void OnEnable()
		{
			EditorApplication.update += Sample;
			Reset();
		}

		private void OnDisable()
		{
			EditorApplication.update -= Sample;
		}

		private void Reset()
		{
			_peakBatches = 0;
			_peakSetPassCalls = 0;
			_minFps = float.MaxValue;
			_worstFrameMs = 0f;
			_fpsSum = 0f;
			_fpsSamples = 0;
			_measureStartTime = EditorApplication.timeSinceStartup;
		}

		private void Sample()
		{
			if (EditorApplication.isPlaying == false)
				return;

			_batches = UnityStats.batches;
			_setPassCalls = UnityStats.setPassCalls;

			float delta = Time.unscaledDeltaTime;

			if (delta > 0f)
				_fps = 1f / delta;

			if (EditorApplication.timeSinceStartup - _measureStartTime > WarmupDuration)
			{
				if (_batches > _peakBatches)
					_peakBatches = _batches;

				if (_setPassCalls > _peakSetPassCalls)
					_peakSetPassCalls = _setPassCalls;

				if (_fps > 0f && _fps < _minFps)
					_minFps = _fps;

				if (delta * 1000f > _worstFrameMs)
					_worstFrameMs = delta * 1000f;

				_fpsSum += _fps;
				_fpsSamples++;
			}

			if (_countObjects && EditorApplication.timeSinceStartup >= _nextCountTime)
			{
				_nextCountTime = EditorApplication.timeSinceStartup + CountInterval;
				CountObjects();
			}

			Repaint();
		}

		private void CountObjects()
		{
			SpriteRenderer[] renderers = FindObjectsByType<SpriteRenderer>(
				FindObjectsInactive.Include,
				FindObjectsSortMode.None);

			_spriteRenderers = renderers.Length;
			_activeSpriteRenderers = 0;

			foreach (SpriteRenderer renderer in renderers)
			{
				if (renderer.gameObject.activeInHierarchy && renderer.enabled)
					_activeSpriteRenderers++;
			}

			_transforms = FindObjectsByType<Transform>(
				FindObjectsInactive.Include,
				FindObjectsSortMode.None).Length;
		}

		private void OnGUI()
		{
			if (EditorApplication.isPlaying == false)
			{
				EditorGUILayout.HelpBox("Enter Play Mode to measure.", MessageType.Info);
				return;
			}

			float average = _fpsSamples > 0 ? _fpsSum / _fpsSamples : 0f;
			float minimum = _minFps < float.MaxValue ? _minFps : 0f;

			EditorGUILayout.LabelField("Frame", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("FPS", $"{_fps:0}");
			EditorGUILayout.LabelField("FPS avg", $"{average:0}");
			EditorGUILayout.LabelField("FPS min", $"{minimum:0.0}");
			EditorGUILayout.LabelField("Worst frame", $"{_worstFrameMs:0.0} ms");

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Rendering", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("Batches", $"{_batches}  (peak {_peakBatches})");
			EditorGUILayout.LabelField("SetPass", $"{_setPassCalls}  (peak {_peakSetPassCalls})");

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Scene", EditorStyles.boldLabel);
			_countObjects = EditorGUILayout.Toggle("Count objects", _countObjects);

			if (_countObjects)
			{
				EditorGUILayout.LabelField("SpriteRenderers", $"{_activeSpriteRenderers} / {_spriteRenderers}");
				EditorGUILayout.LabelField("Transforms", $"{_transforms}");
			}

			EditorGUILayout.Space();

			if (GUILayout.Button("Reset peaks"))
				Reset();
		}
	}
}
