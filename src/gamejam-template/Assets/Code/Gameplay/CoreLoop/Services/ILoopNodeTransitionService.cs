using Code.Infrastructure.CoreLoop;
using Cysharp.Threading.Tasks;

namespace Code.Gameplay.CoreLoop.Services
{
	public interface ILoopNodeTransitionService
	{
		UniTask GoTo(LoopNodeId nodeId);
	}
}
