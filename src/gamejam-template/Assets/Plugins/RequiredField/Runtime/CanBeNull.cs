using System;
using UnityEngine;

namespace Plugins.RequiredField.Runtime
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true)]
	public class CanBeNull : PropertyAttribute
	{
	}
}