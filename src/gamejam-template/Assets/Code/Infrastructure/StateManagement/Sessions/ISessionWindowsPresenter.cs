using Code.Infrastructure.CoreLoop;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.StateManagement.Sessions
{
	public interface ISessionWindowsPresenter
	{
		UniTask Present(LoopNodeId nodeId);
		UniTask Dismiss(LoopNodeId nodeId);
	}
}
