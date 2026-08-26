using System.Threading;
using Cysharp.Threading.Tasks;

namespace Framework.UI.UiManagement.Elements.Windows
{
	public interface IWindowControl
	{
		UniTask Open(bool withAnimation = true, CancellationToken cancellationToken = default);
		UniTask Close(bool withAnimation = true, CancellationToken cancellationToken = default);
	}
}
