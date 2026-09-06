using System;
using System.Diagnostics;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x02001095 RID: 4245
	internal static class EditorOnlyScripts
	{
		// Token: 0x060069E0 RID: 27104 RVA: 0x00002C2D File Offset: 0x00000E2D
		[Conditional("UNITY_EDITOR")]
		public static void Cleanup(GameObject[] rootObjects, bool force = false)
		{
		}
	}
}
