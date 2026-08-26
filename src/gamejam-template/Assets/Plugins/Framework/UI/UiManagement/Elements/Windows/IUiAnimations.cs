using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Framework.UI.UiManagement.Elements.Windows
{
	public interface IUiAnimations
	{
		void Initialize();
		UniTask PlayOpenAnimation(Action onComplete = null, CancellationToken cancellationToken = default);
		UniTask PlayCloseAnimation(Action onComplete = null, CancellationToken cancellationToken = default);
		void PlayIdleAnimation();
	}
}
