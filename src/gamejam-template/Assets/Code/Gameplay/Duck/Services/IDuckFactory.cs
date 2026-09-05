namespace Code.Gameplay.Duck.Services
{
	public interface IDuckFactory
	{
		GameEntity CreateDuck();

		void CreateThrowDuckRequest();
	}
}
