namespace Code.Gameplay.Progress.Queries
{
	public interface IProgressQuery
	{
		string GetPlayerName();
		bool HasSeenIntro();
		int GetBestAnswers();
		float GetBestTimeSeconds();
	}
}
