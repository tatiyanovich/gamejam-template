using System.Collections.Generic;
using Code.Gameplay.Difficulty.Data;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Difficulty.Configs
{
	[CreateAssetMenu(fileName = "DifficultyConfig", menuName = "Configs/Difficulty/DifficultyConfig")]
	public class DifficultyConfig : ScriptableObject
	{
		[SF] private List<DifficultyPhase> phases = new();

		public IReadOnlyList<DifficultyPhase> Phases => phases;
	}
}
