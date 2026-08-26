using System.Collections.Generic;
using Zenject;

namespace Framework.Instantiation
{
	/// <summary>
	/// Bind it in SceneContext so it updates every <see cref="IInstantiatorDependant"/> with the
	/// current SceneContext <see cref="IInstantiator"/>.
	/// </summary>
	public class InstantiatorSetter : IInitializable
	{
		public InstantiatorSetter(IInstantiator instantiator, IEnumerable<IInstantiatorDependant> instantiatorDependants)
		{
			foreach (IInstantiatorDependant instantiatorDependant in instantiatorDependants)
			{
				instantiatorDependant.SetInstantiator(instantiator);
			}
		}

		public void Initialize()
		{
		}
	}
}
