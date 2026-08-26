using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Infrastructure.EntityComponentSystem
{
	public class EntityDependant : MonoBehaviour
	{
		[SF] private EntityView entityView;

		public GameEntity Entity => entityView != null ? entityView.Entity : null;
		public EntityView EntityView => entityView;

		private void Awake()
		{
			if (entityView == false)
				SetEntityView(GetComponent<EntityView>());

			OnAwake();
		}
		
		public void SetEntityView(EntityView view)
		{
			entityView = view;
		}

		protected virtual void OnAwake()
		{
		}
	}
}