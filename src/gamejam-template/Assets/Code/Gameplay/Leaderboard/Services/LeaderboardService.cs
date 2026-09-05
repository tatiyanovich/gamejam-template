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

			if (response == null || response.top == null || string.IsNullOrEmpty(response.error) == false)
			{
				Debug.LogWarning($"Leaderboard response rejected: {json}");
				return LeaderboardResponse.Offline;
			}

			return response.ToResponse();
		}
	}
}
