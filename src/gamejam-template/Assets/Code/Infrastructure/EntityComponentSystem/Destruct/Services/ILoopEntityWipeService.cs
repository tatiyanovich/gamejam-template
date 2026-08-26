using Code.Infrastructure.CoreLoop;

namespace Code.Infrastructure.EntityComponentSystem.Destruct.Services
{
	public interface ILoopEntityWipeService
	{
		void WipeNodeScopedEntities();
		void WipeNodeScopedEntities(LoopNodeId nodeId);
	}
}
