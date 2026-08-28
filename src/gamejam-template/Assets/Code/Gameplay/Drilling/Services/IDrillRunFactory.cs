namespace Code.Gameplay.Drilling.Services
{
	public interface IDrillRunFactory
	{
		GameEntity CreateRun(float bestDistance);
	}
}
