using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.Essentials.ViewManagement;
using UnityEngine;

namespace Framework.UI.UiManagement.Elements.Windows
{
	public abstract class WidgetBase : MonoBehaviour, IUnityView, IDisposable
	{
		private IUiAnimations _uiAnimations;

		public event Action OnWidgetOpen;
		public event Action OnWidgetClose;

		public bool IsOpen { get; private set; }
		public bool IsClosingInProgress { get; private set; }

		public RectTransform RectTransform { get; private set; }
		public string ConfigGuid { get; internal set; }

		/// <summary>
		/// Defines how this widget's lifecycle is managed. By default, widgets are controlled by their parent window.
		/// </summary>
		public virtual WidgetLifecycleOwner LifecycleOwner => WidgetLifecycleOwner.Window;

		/// <summary>Whether this widget is opened/closed by its parent window.</summary>
		public bool IsControlledByWindow => LifecycleOwner == WidgetLifecycleOwner.Window;

		/// <summary>Whether this widget opens on <c>OnEnable</c> and closes on <c>OnDisable</c>.</summary>
		public bool IsControlledByUnityActiveState => LifecycleOwner == WidgetLifecycleOwner.UnityActiveState;

		public WindowBase OwnerWindow { get; private set; }

		protected virtual void Awake()
		{
			RectTransform = (RectTransform)transform;
			_uiAnimations = transform.GetComponent<IUiAnimations>();
			_uiAnimations?.Initialize();
		}

		private void OnEnable()
		{
			OwnerWindow = GetComponentInParent<WindowBase>();

			if (IsControlledByUnityActiveState)
				Open().Forget();
		}

		private void OnDisable()
		{
			if (IsControlledByUnityActiveState)
				Close().Forget();
		}

		private void Update()
		{
			if (IsOpen || IsClosingInProgress)
				OnUpdate();
		}

		public async UniTask Open(bool withAnimation = true, CancellationToken cancellationToken = default)
		{
			if (IsOpen)
				return;

			IsOpen = true;
			gameObject.SetActive(true);

			await OnOpen(cancellationToken);

			if (_uiAnimations != null && withAnimation)
				await _uiAnimations.PlayOpenAnimation(cancellationToken: cancellationToken);

			await OnOpenFinished(cancellationToken);

			OnWidgetOpen?.Invoke();
		}

		public async UniTask Close(bool withAnimation = true, CancellationToken cancellationToken = default)
		{
			if (IsOpen == false)
				return;

			IsOpen = false;
			IsClosingInProgress = true;

			await OnClose(cancellationToken);

			if (_uiAnimations != null && withAnimation)
				await _uiAnimations.PlayCloseAnimation(cancellationToken: cancellationToken);

			gameObject.SetActive(false);
			await OnCloseFinished(cancellationToken);
			IsClosingInProgress = false;
			OnWidgetClose?.Invoke();
		}

		public virtual void Dispose() { }
		protected virtual void OnUpdate() { }
		protected virtual UniTask OnOpen(CancellationToken cancellationToken = default) => UniTask.CompletedTask;
		protected virtual UniTask OnOpenFinished(CancellationToken cancellationToken = default) => UniTask.CompletedTask;
		protected virtual UniTask OnClose(CancellationToken cancellationToken = default) => UniTask.CompletedTask;
		protected virtual UniTask OnCloseFinished(CancellationToken cancellationToken = default) => UniTask.CompletedTask;

		private void OnDestroy() => Dispose();
	}
}
