using UnityEngine;

namespace Framework.UI.UiManagement.Services
{
	public interface IUiHolder
	{
		Transform Root { get; }
		Camera UiCamera { get; }
	}
}
