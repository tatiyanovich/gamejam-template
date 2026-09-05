using Code.Storage.SaveFiles;

namespace Code.Gameplay.Progress.Services
{
	public interface IProgressFactory
	{
		GameEntity CreateExamProgress(GeneralSaveFile saveFile);
		void CreateSetPlayerNameRequest(string playerName);
		void CreateMarkIntroSeenRequest();
	}
}
