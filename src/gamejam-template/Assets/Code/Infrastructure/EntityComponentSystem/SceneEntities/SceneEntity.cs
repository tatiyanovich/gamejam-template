using Entitas;
using Framework.Essentials.ScriptableObjects;
using UnityEngine;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.Infrastructure.EntityComponentSystem.SceneEntities
{
	[RequireComponent(typeof(EntityView))]
	public abstract class SceneEntity : MonoBehaviour
	{
		[SF, ID] private string sceneEntityGuid;
		protected GameContext Game;
		
		public string SceneEntityGuid => sceneEntityGuid;
		
		[Inject]
		private void Construct(GameContext game)
		{
			Game = game;
			Game.CreateEntity().AddInitializeSceneEntityRequest(this);
		}

		/// <summary>
		/// Builds the entity and adds all the necessary components to it.
		/// Called by InitializeSceneEntitiesByRequestSystem when the scene is loaded.
		/// </summary>
		public abstract void Initialize();
		
		/// <summary>
		/// Should be used to resolve references between entities created by scene entities.
		/// Called by InitializeSceneEntitiesByRequestSystem after all scene entities have been initialized.
		/// </summary>
		public abstract void ResolveReferences();
		/// <summary>
		/// ID of the entity created by this scene entity. Should be used to resolve references between entities created by scene entities.
		/// IMPORTANT: This is not the same as the scene entity's guid, which is used to identify the scene entity itself.
		/// It can be accessed only after request is processed, so it can't be used in Initialize() method, but only in ResolveReferences() method.
		/// </summary>
		/// <returns></returns>
		public abstract int Id();
	}
	
	/// <summary>
	/// Should be used to create entities with no view, but still from the scene.
	/// </summary>
	public abstract class SceneEntity<TEntity> : SceneEntity where TEntity : class, IEntity
	{
		public TEntity Entity { get; protected set; }
		
		public override void Initialize()
		{
			Entity = BuildEntity();
			OnInitialized(Entity);
		}
		
		protected virtual void OnInitialized(TEntity entity) { }
		protected abstract TEntity BuildEntity();
	}
}