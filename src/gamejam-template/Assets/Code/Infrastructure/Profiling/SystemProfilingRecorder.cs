using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Code.Infrastructure.Profiling
{
	/// <summary>
	/// Accumulates per-system, per-phase execution timings and writes a sortable report to disk.
	/// Reports are written to <c>Application.persistentDataPath/Profiling/</c> (path is logged to the console).
	/// Timings are cumulative across the session; the file is overwritten on every flush so it always
	/// holds the latest averages. The first <see cref="WarmupFrames"/> frames are skipped to exclude
	/// the load/spawn spike from the averages.
	/// </summary>
	public sealed class SystemProfilingRecorder
	{
		public static readonly SystemProfilingRecorder Instance = new SystemProfilingRecorder();

		/// <summary>Master switch. Set to false to disable all measurement with near-zero overhead.</summary>
		public static bool Enabled = true;

		private const int WarmupFrames = 120;
		private const int FlushEveryFrames = 600;
		private const string ReportFolderName = "Profiling";

		private readonly Dictionary<string, Sample> _samples = new Dictionary<string, Sample>(256);
		private readonly double _ticksToMilliseconds = 1000.0 / System.Diagnostics.Stopwatch.Frequency;

		private int _frameCount;

		public bool IsRecording => Enabled && _frameCount >= WarmupFrames;

		public void Record(string systemName, ProfilePhase phase, long elapsedTicks)
		{
			if (IsRecording == false)
				return;

			string key = BuildKey(systemName, phase);

			if (_samples.TryGetValue(key, out Sample sample) == false)
			{
				sample = new Sample(systemName, phase);
				_samples[key] = sample;
			}

			sample.Add(elapsedTicks);
		}

		public void OnFrameComplete()
		{
			if (Enabled == false)
				return;

			_frameCount++;

			int measuredFrames = _frameCount - WarmupFrames;
			if (measuredFrames > 0 && measuredFrames % FlushEveryFrames == 0)
				WriteReport();
		}

		public void Reset()
		{
			_samples.Clear();
			_frameCount = 0;
		}

		public void WriteReport()
		{
			if (_samples.Count == 0)
				return;

			int measuredFrames = Mathf.Max(1, _frameCount - WarmupFrames);

			string directory = Path.Combine(Application.persistentDataPath, ReportFolderName);
			Directory.CreateDirectory(directory);

			string textPath = Path.Combine(directory, "system_profile.txt");
			string csvPath = Path.Combine(directory, "system_profile.csv");

			File.WriteAllText(textPath, BuildTextReport(measuredFrames));
			File.WriteAllText(csvPath, BuildCsvReport(measuredFrames));

			Debug.Log($"[SystemProfiling] Report written over {measuredFrames} measured frames -> {textPath}");
		}

		private string BuildTextReport(int measuredFrames)
		{
			List<Sample> ordered = _samples.Values.OrderByDescending(sample => sample.TotalTicks).ToList();
			double grandTotalMilliseconds = ordered.Sum(sample => sample.TotalTicks) * _ticksToMilliseconds;

			StringBuilder builder = new StringBuilder();
			builder.AppendLine("System Profiling Report");
			builder.AppendLine($"Generated:        {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
			builder.AppendLine($"Measured frames:  {measuredFrames} (first {WarmupFrames} warmup frames skipped)");
			builder.AppendLine($"Total CPU/frame:  {(grandTotalMilliseconds / measuredFrames):F3} ms (sum of all measured systems)");
			builder.AppendLine();
			builder.AppendLine(string.Format(
				"{0,-34}{1,-11}{2,8}{3,13}{4,12}{5,12}{6,11}{7,8}",
				"System", "Phase", "Calls", "Total ms", "ms/frame", "Avg ms", "Max ms", "Share"));
			builder.AppendLine(new string('-', 109));

			foreach (Sample sample in ordered)
			{
				double totalMilliseconds = sample.TotalTicks * _ticksToMilliseconds;
				double millisecondsPerFrame = totalMilliseconds / measuredFrames;
				double averageMilliseconds = totalMilliseconds / sample.CallCount;
				double maxMilliseconds = sample.MaxTicks * _ticksToMilliseconds;
				double sharePercent = grandTotalMilliseconds > 0.0 ? totalMilliseconds / grandTotalMilliseconds * 100.0 : 0.0;

				builder.AppendLine(string.Format(
					"{0,-34}{1,-11}{2,8}{3,13:F2}{4,12:F3}{5,12:F4}{6,11:F3}{7,7:F1}%",
					Truncate(sample.SystemName, 33),
					sample.Phase,
					sample.CallCount,
					totalMilliseconds,
					millisecondsPerFrame,
					averageMilliseconds,
					maxMilliseconds,
					sharePercent));
			}

			return builder.ToString();
		}

		private string BuildCsvReport(int measuredFrames)
		{
			List<Sample> ordered = _samples.Values.OrderByDescending(sample => sample.TotalTicks).ToList();

			StringBuilder builder = new StringBuilder();
			builder.AppendLine("System,Phase,Calls,TotalMs,MsPerFrame,AvgMs,MaxMs");

			foreach (Sample sample in ordered)
			{
				double totalMilliseconds = sample.TotalTicks * _ticksToMilliseconds;
				double millisecondsPerFrame = totalMilliseconds / measuredFrames;
				double averageMilliseconds = totalMilliseconds / sample.CallCount;
				double maxMilliseconds = sample.MaxTicks * _ticksToMilliseconds;

				builder.AppendLine(string.Format(
					CultureInfo.InvariantCulture,
					"{0},{1},{2},{3:F3},{4:F4},{5:F4},{6:F3}",
					sample.SystemName,
					sample.Phase,
					sample.CallCount,
					totalMilliseconds,
					millisecondsPerFrame,
					averageMilliseconds,
					maxMilliseconds));
			}

			return builder.ToString();
		}

		private static string BuildKey(string systemName, ProfilePhase phase)
		{
			return $"{systemName}#{phase}";
		}

		private static string Truncate(string value, int maxLength)
		{
			if (value.Length <= maxLength)
				return value;

			return value.Substring(0, maxLength - 1) + "…";
		}

		private sealed class Sample
		{
			public string SystemName { get; }
			public ProfilePhase Phase { get; }
			public long CallCount { get; private set; }
			public long TotalTicks { get; private set; }
			public long MaxTicks { get; private set; }

			public Sample(string systemName, ProfilePhase phase)
			{
				SystemName = systemName;
				Phase = phase;
			}

			public void Add(long elapsedTicks)
			{
				CallCount++;
				TotalTicks += elapsedTicks;

				if (elapsedTicks > MaxTicks)
					MaxTicks = elapsedTicks;
			}
		}
	}
}
