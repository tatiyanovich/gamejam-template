using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
using UnityEngine;

namespace Framework.UI.UiManagement.Services
{
	public interface IUiFactory
	{
		UniTask<WindowBase> CreateWindow(WindowConfig config, Transform parent, CancellationToken cancellationToken = default);
		UniTask<WidgetBase> CreateWidget(WidgetConfig config, Transform parent = null, CancellationToken cancellationToken = default);
	}
}
