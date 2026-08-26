using Code.Infrastructure.EntityComponentSystem.Identifiers;
using Code.Storage.SaveFiles;
using Entitas;
using Framework.Storage;

namespace Code.Storage.Systems
{
	public class RefreshAppMetadataSystem : IExecuteSystem
	{
		private readonly ISaveLoadService _saveLoadService;
		private readonly IIdentifierService _identifierService;

		public RefreshAppMetadataSystem(ISaveLoadService saveLoadService, IIdentifierService identifierService)
		{
			_saveLoadService = saveLoadService;
			_identifierService = identifierService;
		}

		public void Execute()
		{
			GeneralSaveFile saveFile = _saveLoadService.Get<GeneralSaveFile>();
			saveFile.AppMetadata.LastUsedId = _identifierService.GetLastUsedId();
		}
	}
}
