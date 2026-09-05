using Code.Gameplay.Greybox.Behaviours;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Greybox.Services
{
	public class GreyboxBoardFactory : IGreyboxBoardFactory
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IIdentifierService _identifierService;

		public GreyboxBoardFactory(IEntityFactory entityFactory, IIdentifierService identifierService)
		{
			_entityFactory = entityFactory;
			_identifierService = identifierService;
		}

		public GameEntity CreateBoard()
		{
			GameObject root = new("Greybox Board");
			root.transform.SetParent(ProjectContext.Instance.transform);

			GreyboxBoard board = root.AddComponent<GreyboxBoard>();

			return _entityFactory.Game()
				.AddId(_identifierService.Next())
				.AddGreyboxBoard(board);
		}
	}
}
