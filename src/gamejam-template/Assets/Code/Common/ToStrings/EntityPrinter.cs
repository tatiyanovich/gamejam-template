using Entitas;

namespace Code.Common.ToStrings
{
	public class EntityPrinter
	{
		private readonly INamedEntity _entity;

		private string _toStringCache;
		private bool _dirty = true;
		private bool _subscribed;

		public EntityPrinter(INamedEntity entity)
		{
			_entity = entity;
		}

		public string BuildToString()
		{
			EnsureSubscribed();

			if (_dirty == false && _toStringCache != null)
				return _toStringCache;

			IComponent[] components = _entity.GetComponents();

			_toStringCache = components.Length == 0
				? "No components"
				: _entity.EntityName(components);

			_dirty = false;

			return _toStringCache;
		}

		private void EnsureSubscribed()
		{
			if (_subscribed)
				return;

			_entity.OnComponentAdded += HandleComponentChanged;
			_entity.OnComponentRemoved += HandleComponentChanged;
			_entity.OnComponentReplaced += HandleComponentReplaced;
			_entity.OnDestroyEntity += HandleDestroy;

			_subscribed = true;
		}

		private void HandleComponentChanged(IEntity entity, int index, IComponent component) => _dirty = true;

		private void HandleComponentReplaced(IEntity entity, int index, IComponent previous, IComponent replacement) =>
			_dirty = true;

		private void HandleDestroy(IEntity entity)
		{
			_subscribed = false;
			_dirty = true;
		}
	}
}
