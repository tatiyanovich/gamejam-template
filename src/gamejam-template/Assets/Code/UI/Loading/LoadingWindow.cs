using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Loading
{
	public class LoadingWindow : WindowBase
	{
		[SF] private ProgressBar progressBar;

		protected override UniTask OnOpen(CancellationToken cancellationToken = new CancellationToken())
		{
			SetProgress(0f);
			return base.OnOpen(cancellationToken);
		}

		public void SetProgress(float progress)
		{
			progressBar.DirectValue = progress;
		}
	}
}