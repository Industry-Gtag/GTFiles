using System;
using UnityEngine;

// Token: 0x020002A4 RID: 676
public static class ApplicationQuittingState
{
	// Token: 0x170001BE RID: 446
	// (get) Token: 0x060011C7 RID: 4551 RVA: 0x0005F716 File Offset: 0x0005D916
	// (set) Token: 0x060011C8 RID: 4552 RVA: 0x0005F71D File Offset: 0x0005D91D
	public static bool IsQuitting { get; private set; }

	// Token: 0x060011C9 RID: 4553 RVA: 0x0005F725 File Offset: 0x0005D925
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		Application.quitting += ApplicationQuittingState.HandleApplicationQuitting;
	}

	// Token: 0x060011CA RID: 4554 RVA: 0x0005F738 File Offset: 0x0005D938
	private static void HandleApplicationQuitting()
	{
		ApplicationQuittingState.IsQuitting = true;
	}
}
