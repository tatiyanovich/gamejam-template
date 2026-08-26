using Code.Infrastructure.CoreLoop;
using Code.Infrastructure.StateManagement.States;

namespace Code.Infrastructure.StateManagement.Sessions
{
	public interface ISessionService
	{
		void EnterNode(LoopScenePayload payload);
		void CloseSession(LoopNodeId nodeId);
		void CloseAll();
	}
}
