using UnityEngine;

namespace Framework.UI.UiManagement.Services
{
	public class UiHolder : MonoBehaviour, IUiHolder
	{
		[SerializeField] private Transform root;
		[SerializeField] private Camera uiCamera;

		public Transform Root => root;
		public Camera UiCamera => uiCamera;
	}
}
