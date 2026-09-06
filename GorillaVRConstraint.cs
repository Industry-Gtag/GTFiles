using System;
using UnityEngine;

// Token: 0x020008E5 RID: 2277
public class GorillaVRConstraint : MonoBehaviourTick
{
	// Token: 0x06003B9E RID: 15262 RVA: 0x00146AF4 File Offset: 0x00144CF4
	public override void Tick()
	{
		if (NetworkSystem.Instance.WrongVersion)
		{
			this.isConstrained = true;
		}
		if (this.isConstrained && Time.realtimeSinceStartup > this.angle)
		{
			GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
		}
	}

	// Token: 0x04004C36 RID: 19510
	public bool isConstrained;

	// Token: 0x04004C37 RID: 19511
	public float angle = 3600f;
}
