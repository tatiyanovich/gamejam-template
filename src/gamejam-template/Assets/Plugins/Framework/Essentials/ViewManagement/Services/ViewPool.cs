using System;
using System.Collections.Generic;
using Framework.Essentials.SceneManagement;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Framework.Essentials.ViewManagement.Services
{
	public class ViewPool : IViewPool, IDisposable
	{
		private readonly ISceneLoadService _sceneLoadService;

		private readonly Dictionary<string, Queue<IUnityView>> _pool = new();
		private readonly Transform _parent;
		private readonly Transform _projectContextTransform;

		public ViewPool(ISceneLoadService sceneLoadService)
		{
			_sceneLoadService = sceneLoadService;

			_projectContextTransform = ProjectContext.Instance.transform;

			_parent = new GameObject("View Pool").transform;
			_parent.SetParent(_projectContextTransform);

			_sceneLoadService.OnSceneLoadStarted += HandleSceneLoadStarted;
		}

		public void Dispose()
		{
			_sceneLoadService.OnSceneLoadStarted -= HandleSceneLoadStarted;
		}

		public IUnityView Get(string viewPath)
		{
			if (viewPath == null)
				throw new ArgumentNullException(nameof(viewPath));

			EnsurePoolEntry(viewPath);

			if (Has(viewPath) == false)
				return null;

			IUnityView view = _pool[viewPath].Dequeue();
			view.transform.SetParent(_projectContextTransform);
			view.gameObject.SetActive(true);

			return view;
		}

		public void Put(IUnityView view, string viewPath)
		{
			if (view == null)
				throw new ArgumentNullException(nameof(view));

			if (viewPath == null)
				throw new ArgumentNullException(nameof(viewPath));

			if (view.gameObject == null)
				return;

			EnsurePoolEntry(viewPath);
			view.transform.SetParent(_parent, false);
			view.gameObject.SetActive(false);
			_pool[viewPath].Enqueue(view);
		}

		public bool Has(string viewPath)
		{
			if (viewPath == null)
				throw new ArgumentNullException(nameof(viewPath));

			return _pool.ContainsKey(viewPath) && _pool[viewPath].Count > 0;
		}

		private void EnsurePoolEntry(string viewPath)
		{
			if (_pool.ContainsKey(viewPath) == false)
				_pool[viewPath] = new Queue<IUnityView>();
		}

		private void HandleSceneLoadStarted()
		{
			foreach (KeyValuePair<string, Queue<IUnityView>> pair in _pool)
			{
				while (pair.Value.Count > 0)
				{
					IUnityView view = pair.Value.Dequeue();
					if (view != null && view.gameObject != null)
						Object.Destroy(view.gameObject);
				}
			}

			_pool.Clear();
		}
	}
}
