namespace Code.Infrastructure.EntityComponentSystem.Identifiers
{
	public interface IIdentifierService
	{
		int Next();
		int GetLastUsedId();
		int SetLastUsedId(int id);
	}
}