namespace Code.Infrastructure.EntityComponentSystem.Factories
{
	public interface IEntityFactory
	{
		GameEntity Game();
		GameEntity Event();
		GameEntity Request();
		InputEntity Input();
	}
}
