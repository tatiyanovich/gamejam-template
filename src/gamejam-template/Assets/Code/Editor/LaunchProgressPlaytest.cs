using System;
using System.IO;
using Code.Gameplay.Progress.Queries;
using Code.Gameplay.Progress.Services;
using Code.Gameplay.Progress.Systems;
using Code.Infrastructure.CoreLoop;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;
using Code.Storage.SaveFiles;
using UnityEditor;

namespace Code.Editor
{
	public static class LaunchProgressPlaytest
	{
		[MenuItem("COPYCAT/QA/Test launch progress")]
		public static void Run()
		{
			GameContext game = new();
			InputContext input = new();
			try
			{
				EntityFactory entities = new(game, input, new LoopNodeContext());
				ProgressFactory factory = new(entities, new IdentifierService());
				ProgressQuery query = new(game);
				GameEntity progress = factory.CreateExamProgress(new GeneralSaveFile());
				if (query.GetPlayerName() != string.Empty)
					throw new InvalidOperationException("A null save name must load as an empty string.");

				factory.CreateSetPlayerNameRequest("B1Kitten42");
				new SetPlayerNameByRequestSystem(game).Execute();
				if (query.GetPlayerName() != "B1Kitten42" || progress.isPersistAcrossLoopNodes == false)
					throw new InvalidOperationException("The submitted name must survive loop transitions.");

				if (game.GetGroup(GameMatcher
					.AllOf(
						GameMatcher.Request,
						GameMatcher.SaveProgressRequest)).count != 1)
					throw new InvalidOperationException("Name submission must request a snapshot save.");

				File.WriteAllText(PlaytestPaths.Get("launch-progress.txt"),
					"PASS null save name\nPASS first name submission\nPASS persistent progress\nPASS save request\nDONE\n");
			}
			finally
			{
				game.DestroyAllEntities();
				input.DestroyAllEntities();
			}
		}
	}
}
