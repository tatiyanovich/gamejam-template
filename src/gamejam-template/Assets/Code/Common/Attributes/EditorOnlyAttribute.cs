using System;
using UnityEngine;

namespace Code.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class EditorOnlyAttribute : PropertyAttribute
	{
	}
}
