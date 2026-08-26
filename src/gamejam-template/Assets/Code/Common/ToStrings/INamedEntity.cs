using Entitas;

namespace Code.Common.ToStrings
{
	public interface INamedEntity : IEntity
	{
		string EntityName(IComponent[] components);
	}
}