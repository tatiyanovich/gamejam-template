using Zenject;

namespace Framework.Instantiation
{
	public interface IInstantiatorDependant
	{
		void SetInstantiator(IInstantiator newInstantiator);
	}
}
