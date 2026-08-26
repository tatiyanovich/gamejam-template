using UnityEngine;

namespace Framework.Essentials.ViewManagement
{
	public interface IUnityView
	{
		GameObject gameObject { get; }
		Transform transform { get; }
	}
}
