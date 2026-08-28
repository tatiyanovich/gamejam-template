using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.UI
{
	public class WorldOverlayWindow : WindowBase
	{
		[SF] private Canvas rootCanvas;
		[SF] private RectTransform widgetsRoot;

		public Canvas RootCanvas => rootCanvas;
		public RectTransform WidgetsRoot => widgetsRoot;

		protected override async UniTask OnClose(CancellationToken cancellationToken = default)
		{
			await CloseSpawnedWidgets(cancellationToken);
			await base.OnClose(cancellationToken);
		}

		private async UniTask CloseSpawnedWidgets(CancellationToken cancellationToken)
		{
			foreach (WidgetBase widget in GetComponentsInChildren<WidgetBase>(includeInactive: true))
			{
				if (widget.IsControlledByWindow || widget.IsOpen == false)
					continue;

				await _uiService.CloseWidget(widget, withAnimation: false, cancellationToken);
			}
		}
	}
}
