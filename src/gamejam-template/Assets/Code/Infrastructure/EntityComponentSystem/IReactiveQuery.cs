namespace Code.Infrastructure.EntityComponentSystem
{
	/// <summary>
	/// Represents a query that reacts to changes in the entities/components it is interested in.
	/// When changes occur, the query can invoke corresponding events.
	/// </summary>
	public interface IReactiveQuery
	{
		void ReactToChanges();
	}
}
