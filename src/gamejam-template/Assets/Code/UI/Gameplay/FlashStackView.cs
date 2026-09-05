using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Gameplay
{
	public class FlashStackView : MonoBehaviour
	{
		[SF] private FlashRowView[] rows;

		private const float FlashSeconds = 0.6f;

		public void Show(string line, Color tint)
		{
			for (int index = rows.Length - 1; index > 0; index--)
			{
				FlashRowView older = rows[index - 1];
				if (older.SecondsLeft > 0f)
					rows[index].Show(older.Line, older.Tint, older.SecondsLeft);
				else
					rows[index].Hide();
			}

			rows[0].Show(line, tint, FlashSeconds);
		}

		public void Clear()
		{
			foreach (FlashRowView row in rows)
				row.Hide();
		}
	}
}
