using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
using UnityEngine;

namespace Framework.UI.UiManagement.Services
{
	public interface IUiService
	{
		UniTask<T> OpenWindow<T>(Action<T> beforeOpen = null, bool withAnimation = true, CancellationToken cancellationToken = default) where T : WindowBase;
		UniTask<WindowBase> OpenWindow(Type windowType, Action<WindowBase> beforeOpen = null, bool withAnimation = true, CancellationToken cancellationToken = default);
		UniTask<WindowBase> OpenWindow(string guid, Action<WindowBase> beforeOpen = null, bool withAnimation = true, CancellationToken cancellationToken = default);
		UniTask<TWindow> OpenWindow<TWindow>(string guid, Action<TWindow> beforeOpen = null, bool withAnimation = true, CancellationToken cancellationToken = default) where TWindow : WindowBase;

		UniTask CloseWindow<T>(bool withAnimation = true, CancellationToken cancellationToken = default) where T : WindowBase;
		UniTask CloseWindow(WindowBase window, bool withAnimation = true, CancellationToken cancellationToken = default);
		UniTask CloseWindow(string guid, bool withAnimation = true, CancellationToken cancellationToken = default);

		UniTask<T> OpenWidget<T>(Transform parent, bool withAnimation = true, Action<T> beforeOpen = null, CancellationToken cancellationToken = default) where T : WidgetBase;
		UniTask<WidgetBase> OpenWidget(string guid, Transform parent, bool withAnimation = true, Action<WidgetBase> beforeOpen = null, CancellationToken cancellationToken = default);
		UniTask<TWidget> OpenWidget<TWidget>(string guid, Transform parent, bool withAnimation = true, Action<TWidget> beforeOpen = null, CancellationToken cancellationToken = default) where TWidget : WidgetBase;

		UniTask CloseWidget(WidgetBase widget, bool withAnimation = true, CancellationToken cancellationToken = default);

		UniTask Back(string layerId, CancellationToken cancellationToken = default);

		bool IsWindowOpen<T>() where T : WindowBase;
		bool IsWindowOpen(WindowBase window);
		bool IsWindowOpen(Type windowType);
		bool IsWindowOpen(string guid);

		T GetWindow<T>() where T : WindowBase;
		WindowBase GetWindow(string guid);
		List<WindowBase> GetOpenedWindowsInLayer(string layer);

		UniTask CloseAllWindows(bool withAnimation = true, CancellationToken cancellationToken = default);
		UniTask CloseAllWindowsInLayer(string layerId, bool withAnimation = true, CancellationToken cancellationToken = default);

		void Cleanup();
	}
}
