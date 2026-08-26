using UnityEngine;
using UnityEngine.AddressableAssets;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Vfx.Configs
{
	[CreateAssetMenu(fileName = "AssetConfig", menuName = "Configs/AssetConfig")]
	public class AssetConfig : ScriptableObject
	{
		[SF] private AssetReference asset;

		public AssetReference Asset => asset;
	}
}
