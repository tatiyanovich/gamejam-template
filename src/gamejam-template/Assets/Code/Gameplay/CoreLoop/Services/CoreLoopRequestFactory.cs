using Code.Infrastructure.CoreLoop;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.StateManagement.States;

namespace Code.Gameplay.CoreLoop.Services
{
	public class CoreLoopRequestFactory : ICoreLoopRequestFactory
	{
		private readonly IEntityFactory _entityFactory;

		public CoreLoopRequestFactory(IEntityFactory entityFactory)
		{
			_entityFactory = entityFactory;
		}

		public void CreateGoToNodeRequest(LoopNodeId loopNodeId)
		{
			_entityFactory.Request()
				.AddGoToLoopNodeRequest(
					new LoopScenePayload(loopNodeId,
						Addresses.SceneNames.GetLoopScene(loopNodeId)));
		}
		
		public void CreateGoToBranchRequest(LoopNodeId loopNodeId)
		{
			_entityFactory.Request()
				.AddGoToBranchRequest(
				new LoopScenePayload(loopNodeId,
					Addresses.SceneNames.GetLoopScene(loopNodeId)));
		}
		
		public void CreateCloseBranchRequest(LoopNodeId loopNodeId)
		{
			_entityFactory.Request()
				.AddCloseBranchRequest(loopNodeId);
		}
	}
}