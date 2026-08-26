using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine.AddressableAssets;

namespace Code.Infrastructure.EntityComponentSystem.Views
{
	[Game, Watched] public class View : IComponent { public EntityView Value; }
	[Game] public class ViewPath : IComponent { public string Value; }
	[Game] public class ViewPrefab : IComponent { public EntityView Value; }
	[Game] public class ViewAssetReference : IComponent { public AssetReference Value; }
	[Game] public class ViewAddressableKey : IComponent { public string Value; }
	[Game] public class LoadingView : IComponent { }

	[Game]
	public class BecameHiddenEvent : IComponent
	{
		public int Id;
	}
}