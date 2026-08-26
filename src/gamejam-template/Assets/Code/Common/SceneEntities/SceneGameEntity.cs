using Code.Infrastructure.EntityComponentSystem.Identifiers;
using Code.Infrastructure.EntityComponentSystem.SceneEntities;
using Zenject;

namespace Code.Common.SceneEntities
{
	public abstract class SceneGameEntity : SceneEntity<GameEntity> 
	{
		private IIdentifierService _identifiers;
		
		private int _id;

		[Inject]
		private void Construct(IIdentifierService identifiers)
		{
			_identifiers = identifiers;
		}

		protected override void OnInitialized(GameEntity entity)
		{
			_id = _identifiers.Next();
			
			entity.AddId(_id);
		}
		
		public override int Id() => _id;
	}
}