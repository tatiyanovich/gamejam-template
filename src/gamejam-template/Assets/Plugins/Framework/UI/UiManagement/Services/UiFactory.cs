using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.Essentials.ViewManagement;
using Framework.Essentials.ViewManagement.Services;
using Framework.UI.UiManagement.Elements.Windows;
using UnityEngine;

namespace Framework.UI.UiManagement.Services
{
	public class UiFactory : IUiFactory
	{
		private readonly IViewFactory _viewFactory;

		public UiFactory(IViewFactory viewFactory)
		{
			_viewFactory = viewFactory;
		}

		public async UniTask<WindowBase> CreateWindow(WindowConfig config, Transform parent, CancellationToken cancellationToken = default)
		{
			IUnityView instantiated = await _viewFactory.CreateViewFromAddressableKey(config.Prefab.AssetGUID, Vector3.zero, cancellationToken);
			RectTransform rectTransform = (RectTransform)instantiated.transform;
			rectTransform.SetParent(parent, false);
			rectTransform.localScale = Vector3.one;
			rectTransform.anchoredPosition = Vector2.zero;

			return instantiated.gameObject.GetComponent<WindowBase>();
		}

		public async UniTask<WidgetBase> CreateWidget(WidgetConfig config, Transform parent = null, CancellationToken cancellationToken = default)
		{
			IUnityView instantiated = await _viewFactory.CreateViewFromAddressableKey(config.Prefab.AssetGUID, Vector3.zero, cancellationToken);
			((RectTransform)instantiated.transform).SetParent(parent, false);

			return instantiated.gameObject.GetComponent<WidgetBase>();
		}
	}
}
