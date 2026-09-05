using System;
using System.Threading;
using Code.Gameplay.Leaderboard.Configs;
using Code.Gameplay.Leaderboard.Data;
using Code.Infrastructure.Satellite;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Gameplay.Leaderboard.Services
{
	public class LeaderboardService : ILeaderboardService
	{
		private readonly ILeaderboardConfigsService _leaderboardConfigsService;
		private readonly ISatelliteService _satelliteService;

		private const string ContentType = "application/json";

		public LeaderboardService(
			ILeaderboardConfigsService leaderboardConfigsService,
			ISatelliteService satelliteService)
		{
			_leaderboardConfigsService = leaderboardConfigsService;
			_satelliteService = satelliteService;
		}

		public async UniTask<LeaderboardResponse> Submit(
			LeaderboardEntry entry,
			CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (_satelliteService.HasConnection() == false)
				return LeaderboardResponse.Offline;

			LeaderboardConfig config = _leaderboardConfigsService.LeaderboardConfig;
			string payload = JsonUtility.ToJson(LeaderboardEntryDto.From(entry));

			try
			{
				using UnityWebRequest request = UnityWebRequest.Post(config.Url, payload, ContentType);
				request.timeout = config.RequestTimeoutSeconds;

				await request.SendWebRequest().WithCancellation(cancellationToken);

				return Parse(request.downloadHandler.text);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception exception)
			{
				Debug.LogWarning($"Leaderboard submit failed: {exception.Message}");
				return LeaderboardResponse.Offline;
			}
		}

		private static LeaderboardResponse Parse(string json)
		{
			LeaderboardResponseDto response = JsonUtility.FromJson<LeaderboardResponseDto>(json);

			if (IsValid(response) == false)
			{
				Debug.LogWarning($"Leaderboard response rejected: {json}");
				return LeaderboardResponse.Offline;
			}

			return response.ToResponse();
		}

		private static bool IsValid(LeaderboardResponseDto response)
		{
			if (response == null || response.top == null || response.top.Length == 0
				|| response.top.Length > 10 || response.total < response.top.Length
				|| response.rank < 1 || response.rank > response.total
				|| string.IsNullOrEmpty(response.error) == false)
				return false;

			foreach (LeaderboardEntryDto entry in response.top)
			{
				if (entry == null || string.IsNullOrWhiteSpace(entry.name) || entry.name.Length > 12
					|| entry.answers < 0 || entry.answers > 12 || float.IsNaN(entry.timeSeconds)
					|| float.IsInfinity(entry.timeSeconds) || entry.timeSeconds < 0f || entry.timeSeconds > 999f)
					return false;

				if (entry.grade is not ("F" or "D" or "C" or "B" or "A" or "A+"))
					return false;
			}

			return true;
		}
	}
}
