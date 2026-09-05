using Code.Gameplay.Neighbours;
using Code.Gameplay.Neighbours.Queries;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Gameplay
{
	public class PawTimerView : MonoBehaviour
	{
		[SF] private Image fill;
		[SF] private Canvas canvas;

		private NeighbourSide _side;
		private INeighbourQuery _query;

		private void OnDestroy() => Unbind();

		public void Bind(INeighbourQuery query)
		{
			Unbind();
			_query = query;
			_side = transform.position.x < 0f ? NeighbourSide.Left : NeighbourSide.Right;
			_query.OnPawChanged += HandlePaw;
			HandlePaw(_side, _query.IsPawLifted(_side), _query.GetPawWindowTimeLeft(_side));
		}

		public void Unbind()
		{
			if (_query != null)
				_query.OnPawChanged -= HandlePaw;
			_query = null;
			canvas.enabled = false;
		}

		private void HandlePaw(NeighbourSide side, bool lifted, float seconds)
		{
			if (side != _side)
				return;

			canvas.enabled = lifted && seconds > 0f;
			fill.fillAmount = lifted ? _query.GetPawWindowProgress(side) : 0f;
		}
	}
}
