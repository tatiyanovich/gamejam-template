using Code.Gameplay.Teacher;
using UnityEngine;

namespace Code.Gameplay.Greybox.Data
{
	public static class GreyboxColors
	{
		public static readonly Color Wall = new(0.16f, 0.18f, 0.22f);
		public static readonly Color Slot = new(0.08f, 0.08f, 0.1f);
		public static readonly Color Paper = new(0.93f, 0.91f, 0.86f);
		public static readonly Color Body = new(0.55f, 0.5f, 0.6f);
		public static readonly Color Paw = new(0.62f, 0.42f, 0.32f);
		public static readonly Color PawWindow = new(0.4f, 0.85f, 0.5f);
		public static readonly Color Eyes = new(0.98f, 0.98f, 0.98f);
		public static readonly Color Kitten = new(0.8f, 0.75f, 0.7f);
		public static readonly Color KittenLeaning = new(0.97f, 0.82f, 0.3f);
		public static readonly Color MeowArmed = new(0.4f, 0.85f, 0.95f);
		public static readonly Color MeowSpent = new(0.35f, 0.4f, 0.45f);
		public static readonly Color Threshold = new(0.9f, 0.25f, 0.25f);
		public static readonly Color Duck = new(0.98f, 0.85f, 0.2f);
		public static readonly Color DuckAway = new(0.55f, 0.5f, 0.25f);
		public static readonly Color Ink = new(0.12f, 0.12f, 0.14f);
		public static readonly Color Chalk = new(0.92f, 0.92f, 0.95f);

		public static readonly Color SuspicionLow = new(0.95f, 0.85f, 0.3f);
		public static readonly Color SuspicionMedium = new(0.95f, 0.55f, 0.2f);
		public static readonly Color SuspicionHigh = new(0.9f, 0.2f, 0.2f);

		public const string DoneInk = "#2E9E4F";
		public const string PendingInk = "#3C3C42";
		public const string NextInk = "#C24A18";

		public static Color OfAttention(TeacherAttention attention)
		{
			return attention switch
			{
				TeacherAttention.Writing => new Color(0.35f, 0.45f, 0.6f),
				TeacherAttention.Turning => new Color(0.95f, 0.85f, 0.3f),
				TeacherAttention.Watching => new Color(0.95f, 0.55f, 0.2f),
				TeacherAttention.Staring => new Color(0.9f, 0.2f, 0.2f),
				TeacherAttention.Alerted => new Color(0.85f, 0.3f, 0.75f),
				TeacherAttention.Distracted => new Color(0.35f, 0.7f, 0.4f),
				_ => Color.grey
			};
		}

		public static Color OfSuspicion(float ratio)
		{
			if (ratio < 0.5f)
				return SuspicionLow;

			if (ratio < 0.8f)
				return SuspicionMedium;

			return SuspicionHigh;
		}
	}
}
