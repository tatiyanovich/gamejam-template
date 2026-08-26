using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.Essentials.ViewManagement;
using Framework.UI.UiManagement.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Button = Framework.UI.UiManagement.Elements.Buttons.Button;

namespace Framework.UI.UiManagement.Elements.Windows
{
	[RequireComponent(typeof(Canvas))]
	[RequireComponent(typeof(GraphicRaycaster))]
	public abstract class WindowBase : MonoBehaviour, IUnityView, IWindowControl
	{
		[SerializeField] private bool overridingSortingOrder;
		[SerializeField] private int sortingOrder;
		
		[SerializeField] private Button closeButton;

		protected List<WidgetBase> _widgets;
		protected IUiService _uiService;
		private IUiAnimations _uiAnimations;
		private IWindowSounds _windowSounds;

		protected virtual bool WaitForWidgetClose => false;

		public GraphicRaycaster GraphicRaycaster { get; private set; }
		public Canvas Canvas { get; private set; }
		public string Layer { get; private set; }
		public string ConfigGuid { get; internal set; }
		public bool OverridingSortingOrder => overridingSortingOrder;

		protected CancellationTokenSource Cts { get; private set; }

		[Inject]
		public void Construct(IUiService uiService)
		{
			_uiService = uiService;
		}
		
		
		public async UniTask Initialize(string layer, string configGuid, CancellationToken cancellationToken = default)
		{	
			Layer = layer;
			ConfigGuid = configGuid;
			
			ResetLifetimeCts();
			
			Canvas = GetComponent<Canvas>();
			GraphicRaycaster = GetComponent<GraphicRaycaster>();
			_widgets = GetComponentsInChildren<WidgetBase>(true).ToList();

			_uiAnimations = transform.GetComponent<IUiAnimations>();
			_uiAnimations?.Initialize();

			_windowSounds = transform.GetComponent<IWindowSounds>();

			if (overridingSortingOrder)
			{
				Canvas.overrideSorting = true;
				Canvas.sortingOrder = sortingOrder;
			}
			
			await OnInitialize(cancellationToken);
		}

		public virtual void Dispose()
		{
		}

		async UniTask IWindowControl.Open(bool withAnimation, CancellationToken cancellationToken)
		{
			using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(Cts.Token, cancellationToken);
			CancellationToken token = linkedCts.Token;

			_widgets = GetComponentsInChildren<WidgetBase>(true).ToList();

			OpenWidgets(withAnimation);

			_windowSounds?.PlayOpenSound();

			await OnOpen(token);

			if (_uiAnimations != null && withAnimation)
				await _uiAnimations.PlayOpenAnimation(cancellationToken: token);

			await OnOpenFinished(token);

			if (closeButton != null)
				closeButton.OnClicked += HandleCloseButtonClick;
		}

		async UniTask IWindowControl.Close(bool withAnimation, CancellationToken cancellationToken)
		{
			if (closeButton != null)
				closeButton.OnClicked -= HandleCloseButtonClick;

			using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(Cts.Token, cancellationToken);
			CancellationToken token = linkedCts.Token;

			_windowSounds?.PlayCloseSound();

			await OnClose(token);
			await CloseWidgets(token);

			if (_uiAnimations != null && withAnimation)
				await _uiAnimations.PlayCloseAnimation(cancellationToken: token);

			await OnCloseFinished(token);
		}

		/// <summary>Override to add custom logic when the window is initialized.</summary>
		protected virtual UniTask OnInitialize(CancellationToken cancellationToken = default) => UniTask.CompletedTask;

		/// <summary>Override to add custom logic when the window is opened.</summary>
		protected virtual UniTask OnOpen(CancellationToken cancellationToken = default) => UniTask.CompletedTask;

		/// <summary>Override to add custom logic when the window finished opening and all animations are complete.</summary>
		protected virtual UniTask OnOpenFinished(CancellationToken cancellationToken = default) => UniTask.CompletedTask;

		/// <summary>Override to add custom logic when the window starts closing.</summary>
		protected virtual UniTask OnClose(CancellationToken cancellationToken = default) => UniTask.CompletedTask;

		/// <summary>Override to add custom logic when the window is completely closed and all animations are finished.</summary>
		protected virtual UniTask OnCloseFinished(CancellationToken cancellationToken = default) => UniTask.CompletedTask;

		/// <summary>Override to add custom logic executed every frame while the window is open.</summary>
		protected virtual void OnUpdate()
		{
		}

		private void Update()
		{
			OnUpdate();
		}

		private void OnDestroy()
		{
			DisposeLifetimeCts();
			Dispose();
		}

		private void ResetLifetimeCts()
		{
			DisposeLifetimeCts();
			Cts = new CancellationTokenSource();
		}

		private void DisposeLifetimeCts()
		{
			if (Cts == null)
				return;

			if (Cts.IsCancellationRequested == false)
				Cts.Cancel();

			Cts.Dispose();
			Cts = null;
		}

		private void OpenWidgets(bool withAnimation)
		{
			RemoveNullWidgets();

			for (int i = 0; i < _widgets?.Count; i++)
			{
				if (_widgets[i].IsControlledByWindow == false)
					continue;

				_widgets[i].Open(withAnimation).Forget();
			}
		}

		private async UniTask CloseWidgets(CancellationToken cancellationToken = default)
		{
			RemoveNullWidgets();

			for (int i = 0; i < _widgets?.Count; i++)
			{
				if (_widgets[i].IsControlledByWindow == false)
					continue;

				if (WaitForWidgetClose)
					await _widgets[i].Close(cancellationToken: cancellationToken);
				else
					_widgets[i].Close(cancellationToken: cancellationToken).Forget();
			}
		}

		private void RemoveNullWidgets()
		{
			for (int i = _widgets.Count; i-- > 0;)
			{
				if (_widgets[i] == null)
					_widgets.RemoveAt(i);
			}
		}

		private void HandleCloseButtonClick()
		{
			_uiService.CloseWindow(this).Forget();
		}
	}
}
