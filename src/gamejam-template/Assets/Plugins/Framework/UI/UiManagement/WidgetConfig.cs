using Framework.Essentials.ScriptableObjects;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Framework.UI.UiManagement
{
	[CreateAssetMenu(menuName = "Framework/UI/Widget Config")]
	public class WidgetConfig : ScriptableObject
	{
		[SerializeField, ID] private string guid;
		[SerializeField] private AssetReferenceGameObject prefab;

		public string Guid => guid;
		public AssetReferenceGameObject Prefab => prefab;

		/// <summary>Test-only factory. Production code should author configs as ScriptableObject assets.</summary>
		public static WidgetConfig CreateForTest(string guid, string prefabAssetGuid = null)
		{
			WidgetConfig config = CreateInstance<WidgetConfig>();
			config.guid = guid;
			config.prefab = new AssetReferenceGameObject(prefabAssetGuid ?? guid);
			return config;
		}
	}
}
