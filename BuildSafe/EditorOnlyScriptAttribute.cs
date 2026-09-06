using System;
using System.Diagnostics;

namespace BuildSafe
{
	// Token: 0x02001094 RID: 4244
	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Class)]
	public class EditorOnlyScriptAttribute : Attribute
	{
	}
}
