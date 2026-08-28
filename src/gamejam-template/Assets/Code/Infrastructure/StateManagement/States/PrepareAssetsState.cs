using Framework.AssetManagement;
using Framework.StateManagement;
using UnityEngine;

namespace Code.Infrastructure.StateManagement.States
{
	public class PrepareAssetsState : IState, IEnter
	{
		private readonly IAssetsService _assets;
		private readonly IGameStateMachine _gameStateMachine;

		public PrepareAssetsState(
			IAssetsService assets,
			IGameStateMachine gameStateMachine)
		{
			_assets = assets;
			_gameStateMachine = gameStateMachine;
		}

		public void Enter()
		{
			PrepareUiAssets();
			PreparePrefabs();

			_gameStateMachine.Enter<ResolveLoopEntryState>();
		}

		private void PrepareUiAssets()
		{
			_assets.GetAssetsByLabel<Object>(Addresses.Labels.UI);
		}

		private void PreparePrefabs()
		{
			_assets.Load<GameObject>(Addresses.PlayerCharacterKey);
			_assets.Load<GameObject>(Addresses.CameraPrefab);
		}
	}
}
