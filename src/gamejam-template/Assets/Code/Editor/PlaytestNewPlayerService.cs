using Code.Gameplay.Progress.Services;

namespace Code.Editor
{
	public class PlaytestNewPlayerService : INewPlayerService
	{
		public int StartCount { get; private set; }

		public void StartNewPlayer() => StartCount++;
	}
}
