using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Code.Gameplay.Leaderboard.Configs;
using Code.Gameplay.Leaderboard.Data;
using Code.Gameplay.Leaderboard.Services;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Editor
{
	public static class LeaderboardPlaytest
	{
		private static bool _running;

		private static string ReportPath => PlaytestPaths.Get("network.txt");

		[MenuItem("COPYCAT/QA/Test leaderboard")]
		public static void Run() => RunAsync().Forget();

		private static async UniTaskVoid RunAsync()
		{
			if (_running)
				return;
			_running = true;
			File.WriteAllText(ReportPath, "C3 UnityWebRequest tests\n");
			LeaderboardConfig config = ScriptableObject.CreateInstance<LeaderboardConfig>();
			PlaytestConnection connection = new();
			LeaderboardService service = new(new PlaytestLeaderboardConfigs(config), connection);
			LeaderboardEntry entry = new("C3Kitten", 9, 80.25f, "B");
			try
			{
				using (UnityWebRequest reset = UnityWebRequest.Get("http://127.0.0.1:18764/reset"))
					await reset.SendWebRequest();

				SetEndpoint(config, "board");
				LeaderboardResponse response = default;
				for (int index = 0; index < 20; index++)
				{
					string name = index % 2 == 0 ? $"🐱Кот<C3Kitten{index}>" : $"C3Kitten{index}TOOLONG";
					response = await service.Submit(new LeaderboardEntry(name, index % 13, 100 - index, "B"));
					Check($"entry {index + 1}", response.IsOffline == false && response.Total == index + 1);
				}
				Check("top ten", response.Top.Count == 10 && response.Rank > 0 && response.Rank <= 20);
				for (int index = 1; index < response.Top.Count; index++)
				{
					LeaderboardEntry previous = response.Top[index - 1];
					LeaderboardEntry current = response.Top[index];
					Check($"sort {index}", previous.Answers > current.Answers
						|| previous.Answers == current.Answers && previous.TimeSeconds <= current.TimeSeconds);
					Check($"name {index}", current.Name.Length <= 12
						&& System.Text.RegularExpressions.Regex.IsMatch(current.Name, "^[A-Za-z0-9]+$"));
				}

				connection.Connected = false;
				Stopwatch clock = Stopwatch.StartNew();
				response = await service.Submit(entry);
				Check("disconnected immediate", response.IsOffline && clock.Elapsed.TotalSeconds < 1);
				using (CancellationTokenSource cancelled = new())
				{
					cancelled.Cancel();
					bool propagated = false;
					try { await service.Submit(entry, cancelled.Token); }
					catch (OperationCanceledException) { propagated = true; }
					Check("cancelled while offline", propagated);
				}
				connection.Connected = true;

				foreach (string endpoint in new[] { "error", "invalid", "failure", "null", "empty", "negative" })
				{
					SetEndpoint(config, endpoint);
					response = await service.Submit(entry);
					Check(endpoint + " returns offline", response.IsOffline);
				}

				SetEndpoint(config, "slow");
				clock.Restart();
				response = await service.Submit(entry);
				Check("five second timeout", response.IsOffline && clock.Elapsed.TotalSeconds >= 4
					&& clock.Elapsed.TotalSeconds < 7);
				using (CancellationTokenSource cancellation = new())
				{
					cancellation.CancelAfter(200);
					clock.Restart();
					bool propagated = false;
					try { await service.Submit(entry, cancellation.Token); }
					catch (OperationCanceledException) { propagated = true; }
					Check("inflight cancellation", propagated && clock.Elapsed.TotalSeconds < 2);
				}
				SetEndpoint(config, "board");
				response = await service.Submit(entry);
				Check("recovery after timeout", response.IsOffline == false && response.Total == 21);
				File.AppendAllText(ReportPath, "DONE\n");
			}
			catch (Exception exception)
			{
				File.AppendAllText(ReportPath, exception + "\n");
			}
			finally
			{
				_running = false;
				UnityEngine.Object.DestroyImmediate(config);
			}
		}

		private static void SetEndpoint(LeaderboardConfig config, string endpoint)
		{
			SerializedObject serialized = new(config);
			serialized.FindProperty("url").stringValue = "http://127.0.0.1:18764/" + endpoint;
			serialized.ApplyModifiedPropertiesWithoutUndo();
		}

		private static void Check(string name, bool passed)
		{
			File.AppendAllText(ReportPath, $"{(passed ? "PASS" : "FAIL")} {name}\n");
		}
	}
}
