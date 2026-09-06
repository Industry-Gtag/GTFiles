using System;
using UnityEngine;

// Token: 0x020005E8 RID: 1512
public class GorillaTriggerBoxTeleport : GorillaTriggerBox
{
	// Token: 0x060025C4 RID: 9668 RVA: 0x000C8905 File Offset: 0x000C6B05
	public override void OnBoxTriggered()
	{
		this.cameraOffest.GetComponent<Rigidbody>().linearVelocity = new Vector3(0f, 0f, 0f);
		this.cameraOffest.transform.position = this.teleportLocation;
	}

	// Token: 0x0400315E RID: 12638
	public Vector3 teleportLocation;

	// Token: 0x0400315F RID: 12639
	public GameObject cameraOffest;
}
