using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.Essentials.ViewManagement.Services;
using Framework.UI.UiManagement.Elements.Windows;
using UnityEngine;
using UnityEngine.Pool;

namespace Framework.UI.UiManagement.Services
{
	public class UiService : IUiService, IDisposable
	{
		private readonly IUiFactory _uiFactory;
		private readonly IUiHolder _uiHolder;
		private readonly IUiDefinitionService _uiDefinitionService;
		private readonly IViewPool _viewPool;
		private readonly IWindowLayers _layers;
		private readonly IWindowHistory _history;
		private readonly IWindowSideEffectsService _sideEffects;

		public UiService(
			IUiFactory uiFactory,
			IUiHolder uiHolder,
			IUiDefinitionService uiDefinitionService,
			IViewPool viewPool,
			IWindowLayers layers,
			IWindowHistory history,
			IWindowSideEffectsService sideEffects)
		{
			_uiFactory = uiFactory;
			_uiHolder = uiHolder;
			_uiDefinitionService = uiDefinitionService;
			_viewPool = viewPool;
			_layers = layers;
			_history = history;
			_sideEffects = sideEffects;
		}

		public void Dispose() => Cleanup();

		public void Cleanup()
		{
			foreach (string layerId in _layers.Layers)
			foreach (WindowBase window in _layers.Snapshot(layerId))
			{
				try
				{
					_sideEffects.Release(window);
					window.Dispose();
					_viewPool.Put(window, _uiDefinitionService.GetWindowConfig(window.ConfigGuid).Prefab.AssetGUID);
				}
				catch (Exception e)
				{
					Debug.LogError($"Error during cleanup of window {window.GetType().Name} in layer {layerId}: {e.Message}");
				}
			}
		}

		public async UniTask<T> OpenWindow<T>(Action<T> beforeOpen = null, bool withAnimation = true, CancellationToken cancellationToken = default) where T : WindowBase
		{
			WindowConfig config = _uiDefinitionService.GetWindowConfig(typeof(T));
			WindowBase window = await OpenWindowInternal(config, w => beforeOpen?.Invoke((T)w), withAnimation, cancellationToken);
			return (T)window;
		}

		public async UniTask<WindowBase> OpenWindow(Type windowType, Action<WindowBase> beforeOpen = null, bool withAnimation = true, CancellationToken cancellationToken = default)
		{
			WindowConfig config = _uiDefinitionService.GetWindowConfig(windowType);
			return await OpenWindowInternal(config, beforeOpen, withAnimation, cancellationToken);
		}

		public async UniTask<WindowBase> OpenWindow(string guid, Action<WindowBase> beforeOpen = null, bool withAnimation = true, CancellationToken cancellationToken = default)
		{
			WindowConfig config = _uiDefinitionService.GetWindowConfig(guid);
			return await OpenWindowInternal(config, beforeOpen, withAnimation, cancellationToken);
		}

		public async UniTask<TWindow> OpenWindow<TWindow>(string guid, Action<TWindow> beforeOpen = null, bool withAnimation = true, CancellationToken cancellationToken = default) where TWindow : WindowBase
		{
			Action<WindowBase> wrapped = beforeOpen == null ? null : w => beforeOpen((TWindow)w);
			WindowBase window = await OpenWindow(guid, wrapped, withAnimation, cancellationToken);
			return (TWindow)window;
		}

		public async UniTask CloseWindow<T>(bool withAnimation = true, CancellationToken cancellationToken = default) where T : WindowBase
		{
			WindowConfig config = _uiDefinitionService.GetWindowConfig(typeof(T));
			await CloseWindowByConfig(config, withAnimation, cancellationToken);
		}

		public async UniTask CloseWindow(WindowBase window, bool withAnimation = true, CancellationToken cancellationToken = default)
		{
			if (window == null)
				throw new ArgumentNullException(nameof(window), "Window cannot be null.");

			WindowConfig config = _uiDefinitionService.GetWindowConfig(window.ConfigGuid);
			await CloseWindowInternal(window, config.LayerId, removeFromHistory: true, withAnimation, cancellationToken);
			await OpenPreviousHiddenWindow(config.LayerId, cancellationToken);
		}

		public async UniTask CloseWindow(string guid, bool withAnimation = true, CancellationToken cancellationToken = default)
		{
			WindowConfig config = _uiDefinitionService.GetWindowConfig(guid);
			await CloseWindowByConfig(config, withAnimation, cancellationToken);
		}

		public async UniTask<T> OpenWidget<T>(Transform parent, bool withAnimation = true, Action<T> beforeOpen = null, CancellationToken cancellationToken = default) where T : WidgetBase
		{
			WidgetConfig config = _uiDefinitionService.GetWidgetConfig(typeof(T));
			WidgetBase widget = await CreateAndOpenWidget(config, parent, w => beforeOpen?.Invoke((T)w), withAnimation, cancellationToken);
			return (T)widget;
		}

		public async UniTask<WidgetBase> OpenWidget(string guid, Transform parent, bool withAnimation = true, Action<WidgetBase> beforeOpen = null, CancellationToken cancellationToken = default)
		{
			WidgetConfig config = _uiDefinitionService.GetWidgetConfig(guid);
			return await CreateAndOpenWidget(config, parent, beforeOpen, withAnimation, cancellationToken);
		}

		public async UniTask<TWidget> OpenWidget<TWidget>(string guid, Transform parent, bool withAnimation = true, Action<TWidget> beforeOpen = null, CancellationToken cancellationToken = default) where TWidget : WidgetBase
		{
			Action<WidgetBase> wrapped = beforeOpen == null ? null : w => beforeOpen((TWidget)w);
			WidgetBase widget = await OpenWidget(guid, parent, withAnimation, wrapped, cancellationToken);
			return (TWidget)widget;
		}

		public async UniTask CloseWidget(WidgetBase widget, bool withAnimation = true, CancellationToken cancellationToken = default)
		{
			if (widget == null)
				throw new ArgumentNullException(nameof(widget), "Widget cannot be null.");

			string address = _uiDefinitionService.GetWidgetConfig(widget.ConfigGuid).Prefab.AssetGUID;

			await widget.Close(withAnimation, cancellationToken);
			_viewPool.Put(widget, address);
		}

		public bool IsWindowOpen<T>() where T : WindowBase => IsWindowOpen(typeof(T));

		public bool IsWindowOpen(WindowBase window)
		{
			if (window == null)
				throw new ArgumentNullException(nameof(window), "Window cannot be null.");

			WindowConfig config = _uiDefinitionService.GetWindowConfig(window.ConfigGuid);
			return _layers.Find(config.LayerId, window.ConfigGuid) != null;
		}

		public bool IsWindowOpen(Type windowType)
		{
			WindowConfig config = _uiDefinitionService.GetWindowConfig(windowType);
			return _layers.Find(config.LayerId, config.Guid) != null;
		}

		public bool IsWindowOpen(string guid)
		{
			WindowConfig config = _uiDefinitionService.GetWindowConfig(guid);
			return _layers.Find(config.LayerId, guid) != null;
		}

		public T GetWindow<T>() where T : WindowBase
		{
			WindowConfig config = _uiDefinitionService.GetWindowConfig(typeof(T));
			return (T)_layers.Find(config.LayerId, config.Guid);
		}

		public WindowBase GetWindow(string guid)
		{
			WindowConfig config = _uiDefinitionService.GetWindowConfig(guid);
			return _layers.Find(config.LayerId, guid);
		}

		public async UniTask Back(string layerId, CancellationToken cancellationToken = default)
		{
			WindowBase top = _layers.GetTop(layerId);
			if (top == null)
				return;

			WindowConfig config = _uiDefinitionService.GetWindowConfig(top.ConfigGuid);
			if (config.IgnoreBack)
				return;

			await CloseWindowInternal(top, layerId, removeFromHistory: false, withAnimation: true, cancellationToken);
			await OpenPreviousHiddenWindow(layerId, cancellationToken);
		}

		public async UniTask CloseAllWindows(bool withAnimation = true, CancellationToken cancellationToken = default)
		{
			List<UniTask> closeTasks = ListPool<UniTask>.Get();

			foreach (string layerId in _layers.Layers)
			foreach (WindowBase window in _layers.Snapshot(layerId))
				closeTasks.Add(CloseWindowInternal(window, layerId, removeFromHistory: true, withAnimation, cancellationToken));

			await UniTask.WhenAll(closeTasks);
			ListPool<UniTask>.Release(closeTasks);
		}

		public async UniTask CloseAllWindowsInLayer(string layerId, bool withAnimation = true, CancellationToken cancellationToken = default)
		{
			if (_layers.IsRegistered(layerId) == false)
				return;

			List<UniTask> closeTasks = ListPool<UniTask>.Get();

			foreach (WindowBase window in _layers.Snapshot(layerId))
				closeTasks.Add(CloseWindowInternal(window, layerId, removeFromHistory: true, withAnimation, cancellationToken));

			await UniTask.WhenAll(closeTasks);
			ListPool<UniTask>.Release(closeTasks);
		}

		public List<WindowBase> GetOpenedWindowsInLayer(string layer)
		{
			if (string.IsNullOrEmpty(layer))
				throw new ArgumentException("Layer ID cannot be null or empty.", nameof(layer));

			if (_layers.IsRegistered(layer) == false)
				throw new ArgumentException($"Layer {layer} is not registered.", nameof(layer));

			return _layers.Snapshot(layer);
		}

		private async UniTask<WindowBase> OpenWindowInternal(WindowConfig config, Action<WindowBase> beforeOpen, bool withAnimation, CancellationToken cancellationToken)
		{
			ValidateLayer(config.LayerId);

			// A window cannot cover itself. Without this guard, opening a CloseOnCover window
			// that is already on top would push its own guid onto the stack and recreate itself.
			WindowBase currentTop = _layers.GetTop(config.LayerId);
			if (currentTop != null && currentTop.ConfigGuid == config.Guid)
				return currentTop;

			await CoverCurrentTopIfNeeded(currentTop, config.LayerId, withAnimation, cancellationToken);

			WindowBase existing = _layers.Find(config.LayerId, config.Guid);
			if (existing != null)
			{
				_layers.BringToFront(config.LayerId, existing);
				return existing;
			}

			return await CreateAndOpenWindow(config, beforeOpen, withAnimation, cancellationToken);
		}

		private void ValidateLayer(string layerId)
		{
			if (_layers.IsRegistered(layerId) == false)
				throw new ArgumentException($"Layer {layerId} is not registered.");
		}

		private async UniTask CoverCurrentTopIfNeeded(WindowBase currentTop, string layerId, bool withAnimation, CancellationToken cancellationToken)
		{
			if (currentTop == null)
				return;

			WindowConfig topConfig = _uiDefinitionService.GetWindowConfig(currentTop.ConfigGuid);
			if (topConfig.CloseOnCover == false)
				return;

			_history.Push(layerId, currentTop.ConfigGuid);
			await CloseWindowInternal(currentTop, layerId, removeFromHistory: false, withAnimation, cancellationToken);
		}

		private async UniTask<WindowBase> CreateAndOpenWindow(WindowConfig config, Action<WindowBase> beforeOpen, bool withAnimation, CancellationToken cancellationToken)
		{
			WindowBase window = await _uiFactory.CreateWindow(config, _uiHolder.Root, cancellationToken);
			if (window == null)
				throw new InvalidOperationException($"WindowConfig with guid {config.Guid} prefab has no WindowBase component.");

			_layers.Add(config.LayerId, window);
			await window.Initialize(config.LayerId, config.Guid, cancellationToken);
			_layers.UpdateSorting(config.LayerId);

			beforeOpen?.Invoke(window);
			_sideEffects.Apply(window, config);
			await ((IWindowControl)window).Open(withAnimation, cancellationToken);

			return window;
		}

		private async UniTask<WidgetBase> CreateAndOpenWidget(WidgetConfig config, Transform parent, Action<WidgetBase> beforeOpen, bool withAnimation, CancellationToken cancellationToken)
		{
			WidgetBase widget = await _uiFactory.CreateWidget(config, parent, cancellationToken);
			if (widget == null)
				throw new InvalidOperationException($"WidgetConfig with guid {config.Guid} prefab has no WidgetBase component.");

			widget.ConfigGuid = config.Guid;

			beforeOpen?.Invoke(widget);
			await widget.Open(withAnimation, cancellationToken);

			return widget;
		}

		private async UniTask CloseWindowByConfig(WindowConfig config, bool withAnimation, CancellationToken cancellationToken)
		{
			WindowBase window = _layers.Find(config.LayerId, config.Guid);
			if (window == null)
				return;

			await CloseWindowInternal(window, config.LayerId, removeFromHistory: true, withAnimation, cancellationToken);
			await OpenPreviousHiddenWindow(config.LayerId, cancellationToken);
		}

		private async UniTask CloseWindowInternal(WindowBase window, string layerId, bool removeFromHistory, bool withAnimation, CancellationToken cancellationToken = default)
		{
			if (_layers.Contains(layerId, window) == false)
				return;

			_layers.Remove(layerId, window);
			string address = _uiDefinitionService.GetWindowConfig(window.ConfigGuid).Prefab.AssetGUID;
			_sideEffects.Release(window);
			await ((IWindowControl)window).Close(withAnimation, cancellationToken);

			_viewPool.Put(window, address);

			if (removeFromHistory)
				_history.RemoveAll(layerId, window.ConfigGuid);
		}

		private async UniTask OpenPreviousHiddenWindow(string layerId, CancellationToken cancellationToken = default)
		{
			if (_history.TryPop(layerId, out string previousGuid))
				await OpenWindow(previousGuid, cancellationToken: cancellationToken);
		}
	}
}
