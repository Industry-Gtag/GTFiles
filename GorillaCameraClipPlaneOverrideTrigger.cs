using System;
using UnityEngine;

// Token: 0x020005D7 RID: 1495
public class GorillaCameraClipPlaneOverrideTrigger : GorillaTriggerBox
{
	// Token: 0x06002596 RID: 9622 RVA: 0x000C8395 File Offset: 0x000C6595
	private void Awake()
	{
		this.mainCamera = Camera.main;
	}

	// Token: 0x06002597 RID: 9623 RVA: 0x000C83A2 File Offset: 0x000C65A2
	public override void OnBoxTriggered()
	{
		this.mainCamera.farClipPlane = this.clipPlaneFarDistanceOverride;
	}

	// Token: 0x04003102 RID: 12546
	private Camera mainCamera;

	// Token: 0x04003103 RID: 12547
	public float clipPlaneFarDistanceOverride;
}
