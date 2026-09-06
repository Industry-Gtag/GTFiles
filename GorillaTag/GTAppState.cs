using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020011EF RID: 4591
	public static class GTAppState
	{
		// Token: 0x17000B51 RID: 2897
		// (get) Token: 0x060074A9 RID: 29865 RVA: 0x0025E7F3 File Offset: 0x0025C9F3
		// (set) Token: 0x060074AA RID: 29866 RVA: 0x0025E7FA File Offset: 0x0025C9FA
		public static bool isQuitting { get; private set; }

		// Token: 0x060074AB RID: 29867 RVA: 0x0025E804 File Offset: 0x0025CA04
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void HandleOnSubsystemRegistration()
		{
			GTAppState.isQuitting = false;
			Application.quitting += delegate
			{
				GTAppState.isQuitting = true;
			};
			Debug.Log(string.Concat(new string[]
			{
				"GTAppState:\n- SystemInfo.operatingSystem=",
				SystemInfo.operatingSystem,
				"\n- SystemInfo.maxTextureArraySlices=",
				SystemInfo.maxTextureArraySlices.ToString(),
				"\n"
			}));
		}

		// Token: 0x060074AC RID: 29868 RVA: 0x00002C2D File Offset: 0x00000E2D
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void HandleOnAfterSceneLoad()
		{
		}
	}
}
