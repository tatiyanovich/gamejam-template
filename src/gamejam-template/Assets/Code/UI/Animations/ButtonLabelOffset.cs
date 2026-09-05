using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Animations
{
	public sealed class ButtonLabelOffset : Button
	{
		[SerializeField] private RectTransform label;

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);

			if (label != null)
				label.anchoredPosition = state == SelectionState.Pressed ? new Vector2(0f, -8f) : Vector2.zero;
		}
	}
}
