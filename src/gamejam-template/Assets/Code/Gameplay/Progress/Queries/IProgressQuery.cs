namespace Code.Gameplay.Progress.Queries
{
	public interface IProgressQuery
	{
		string GetPlayerName();
		int GetBestAnswers();
		float GetBestTimeSeconds();
	}
}
