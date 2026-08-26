using Code.Infrastructure.EntityComponentSystem.Physics;
using Entitas.Unity;
using Framework.Essentials.ViewManagement;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.EntityComponentSystem
{
	public class EntityView : MonoBehaviour, IUnityView
	{
		private GameEntity _entity;
		private bool _isRegistered;
		private Vector3 _baseScale = Vector3.one;
		private IEntityCollidersRegistry _entityCollidersRegistry;

		public GameEntity Entity => _entity;
		public Vector3 BaseScale => _baseScale;

		[Inject]
		private void Construct(IEntityCollidersRegistry entityCollidersRegistry)
		{
			_entityCollidersRegistry = entityCollidersRegistry;
		}

		private void OnEnable()
		{
			if (_entity == null)
				return;

			_entity.isHidden = false;
		}

		private void OnDisable()
		{
			if (_entity == null)
				return;

			_entity.isHidden = true;
		}

		public void CaptureBaseScale()
		{
			_baseScale = transform.localScale;
		}

		public void AttachEntity(GameEntity entity)
		{
			if (_isRegistered)
				return;

			_entity = entity;
			_entity.AddView(this);
			_entity.ReplaceTransform(transform);

			gameObject.Link(_entity);

			foreach (EntityComponentProvider provider in GetComponentsInChildren<EntityComponentProvider>())
			{
				if (provider.EntityView == this)
					provider.RegisterComponents();
			}

			foreach (Collider foundCollider in GetComponentsInChildren<Collider>(includeInactive: true))
			{
				_entityCollidersRegistry.Register(foundCollider.GetInstanceID(), _entity);
			}

			foreach (Collider2D foundCollider in GetComponentsInChildren<Collider2D>(includeInactive: true))
			{
				_entityCollidersRegistry.Register(foundCollider.GetInstanceID(), _entity);
			}

			_entity.isHidden = gameObject.activeInHierarchy == false;

			_isRegistered = true;
		}

		public void DetachEntity()
		{
			if (_isRegistered == false)
				return;

			foreach (EntityComponentProvider registrar in GetComponentsInChildren<EntityComponentProvider>())
			{
				if (registrar.EntityView == this)
					registrar.UnregisterComponents();
			}

			foreach (Collider foundCollider in GetComponentsInChildren<Collider>(includeInactive: true))
			{
				_entityCollidersRegistry.Unregister(foundCollider.GetInstanceID());
			}

			foreach (Collider2D foundCollider in GetComponentsInChildren<Collider2D>(includeInactive: true))
			{
				_entityCollidersRegistry.Unregister(foundCollider.GetInstanceID());
			}

			_entity.SafeRemoveTransform();
			
			gameObject.GetEntityLink().Unlink();
			_entity = null;

			_isRegistered = false;
		}
	}
}
