namespace Code.Infrastructure.EntityComponentSystem.Identifiers
{
	/// <summary>
	/// Identifier service to provide unique IDs for entities
	/// </summary>
	public class IdentifierService : IIdentifierService
	{
		private int _lastId;

		public int Next() => _lastId += 1;
		public int GetLastUsedId() => _lastId;

		public int SetLastUsedId(int id)
		{
			_lastId = id;
			return _lastId;
		}
	}
}