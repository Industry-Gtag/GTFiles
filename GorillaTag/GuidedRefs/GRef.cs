using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x0200125F RID: 4703
	public static class GRef
	{
		// Token: 0x06007710 RID: 30480 RVA: 0x002699F4 File Offset: 0x00267BF4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ShouldResolveNow(GRef.EResolveModes mode)
		{
			return Application.isPlaying && (mode & GRef.EResolveModes.Runtime) == GRef.EResolveModes.Runtime;
		}

		// Token: 0x06007711 RID: 30481 RVA: 0x00269A05 File Offset: 0x00267C05
		public static bool IsAnyResolveModeOn(GRef.EResolveModes mode)
		{
			return mode > GRef.EResolveModes.None;
		}

		// Token: 0x02001260 RID: 4704
		[Flags]
		public enum EResolveModes
		{
			// Token: 0x040086B5 RID: 34485
			None = 0,
			// Token: 0x040086B6 RID: 34486
			Runtime = 1,
			// Token: 0x040086B7 RID: 34487
			SceneProcessing = 2
		}
	}
}
