namespace Code.Gameplay.Neighbours.Services
{
	public interface INeighbourFactory
	{
		GameEntity CreateNeighbour(NeighbourSide side);
	}
}
