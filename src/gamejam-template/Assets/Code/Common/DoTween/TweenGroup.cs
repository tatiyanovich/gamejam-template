using System.Collections.Generic;
using DG.Tweening;

namespace Code.Common.DoTween
{
	public class TweenGroup
	{
		private readonly List<Tween> _group = new();

		public void Add(Tween tween)
		{
			_group.Add(tween);
		}

		public void Kill(bool complete = false)
		{
			for (int i = 0; i < _group.Count; i++)
			{
				_group[i]?.Kill(complete);
			}

			_group.Clear();
		}
	}
}