using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;
using Code.Storage.SaveFiles;

namespace Code.Gameplay.Progress.Services
{
	public class ProgressFactory : IProgressFactory
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IIdentifierService _identifierService;

		public ProgressFactory(
			IEntityFactory entityFactory,
			IIdentifierService identifierService)
		{
			_entityFactory = entityFactory;
			_identifierService = identifierService;
		}

		public GameEntity CreateExamProgress(GeneralSaveFile saveFile)
		{
			return _entityFactory.Game()
				.AddId(_identifierService.Next())
				.With(x => x.isExamProgress = true)
				.With(x => x.isPersistAcrossLoopNodes = true)
				.AddPlayerName(saveFile.PlayerName ?? string.Empty)
				.AddBestAnswers(saveFile.BestAnswers)
				.AddBestTimeSeconds(saveFile.BestTimeSeconds)
				.With(x => x.isIntroSeen = true, saveFile.IntroSeen);
		}

		public void CreateSetPlayerNameRequest(string playerName)
		{
			_entityFactory.Request()
				.AddSetPlayerNameRequest(playerName);

			_entityFactory.Request()
				.With(x => x.isSaveProgressRequest = true);
		}

		public void CreateMarkIntroSeenRequest()
		{
			_entityFactory.Request()
				.With(x => x.isMarkIntroSeenRequest = true);
		}
	}
}
