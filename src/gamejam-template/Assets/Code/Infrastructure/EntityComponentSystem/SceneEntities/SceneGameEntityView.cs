using Code.Infrastructure.EntityComponentSystem.Identifiers;
using Framework.Storage;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.Infrastructure.EntityComponentSystem.SceneEntities
{
	public abstract class SceneGameEntityView : SceneEntity<GameEntity>
	{
		[SF] private EntityView entityView;

		private IIdentifierService _identifiers;
		private int _id;

		[Inject]
		private void Construct(
			IIdentifierService identifiers,
			ISaveLoadService saveLoadService)
		{
			_identifiers = identifiers;
		}

		protected override void OnInitialized(GameEntity entity)
		{
			if (entityView == null)
			{
				Debug.LogError($"EntityView is not assigned for {gameObject.name}", this);
				return;
			}
			
			_id = _identifiers.Next();
			entity.AddId(_id);
			entity.AddName(gameObject.name);
			entity.AddSceneEntityGuid(SceneEntityGuid);

			entityView.AttachEntity(entity);
		}
		
		public override int Id() => _id;
		
		#if UNITY_EDITOR
		private void Reset()
		{
			SerializedObject serializedObject = new(this);
			SerializedProperty entityViewProperty = serializedObject.FindProperty("entityView");

			if (entityViewProperty != null && entityViewProperty.objectReferenceValue == null)
				entityViewProperty.objectReferenceValue = GetComponent<EntityView>();

			serializedObject.ApplyModifiedPropertiesWithoutUndo();
		}
		#endif
	}
}
