using Code.Gameplay;
using Code.Gameplay.CoreLoop;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.StateManagement.Branching;
using Framework.StateManagement;

namespace Code.Infrastructure.StateManagement.States
{
	public class SessionsRunningState : IState, IEnter, IExecutable, IFixedExecutable, ILateExecutable, IExit
	{
		private readonly ISystemFactory _systemFactory;
		private readonly IBranchedStateMachine _branchedStateMachine;

		private GlobalLoopInfraHeadFeature _infraHead;
		private GlobalLoopInfraTailFeature _infraTail;
		private FixedUpdateFeature _fixedUpdateFeature;
		private LateUpdateFeature _lateUpdateFeature;

		public SessionsRunningState(
			ISystemFactory systemFactory,
			IBranchedStateMachine branchedStateMachine)
		{
			_systemFactory = systemFactory;
			_branchedStateMachine = branchedStateMachine;
		}

		public void Enter()
		{
			_infraHead = _systemFactory.Create<GlobalLoopInfraHeadFeature>();
			_infraTail = _systemFactory.Create<GlobalLoopInfraTailFeature>();
			_fixedUpdateFeature = _systemFactory.Create<FixedUpdateFeature>();
			_lateUpdateFeature = _systemFactory.Create<LateUpdateFeature>();

			_infraHead.Initialize();
			_infraTail.Initialize();
			_fixedUpdateFeature.Initialize();
			_lateUpdateFeature.Initialize();
		}

		public void Execute()
		{
			_infraHead.Execute();
			_infraHead.Cleanup(); // TODO:Check in feature is it broken??

			_branchedStateMachine.ExecuteBranches();

			_infraTail.Execute();
			_infraTail.Cleanup();
		}

		public void FixedExecute()
		{
			_fixedUpdateFeature.Execute();
			_fixedUpdateFeature.Cleanup();

			_branchedStateMachine.FixedExecuteBranches();
		}

		public void LateExecute()
		{
			_lateUpdateFeature.Execute();
			_lateUpdateFeature.Cleanup();

			_branchedStateMachine.LateExecuteBranches();
		}

		public void Exit()
		{
			_infraHead.Cleanup();
			_infraHead.TearDown();
			_infraHead = null;

			_infraTail.Cleanup();
			_infraTail.TearDown();
			_infraTail = null;

			_fixedUpdateFeature.Cleanup();
			_fixedUpdateFeature.TearDown();
			_fixedUpdateFeature = null;

			_lateUpdateFeature.Cleanup();
			_lateUpdateFeature.TearDown();
			_lateUpdateFeature = null;
		}
	}
}
