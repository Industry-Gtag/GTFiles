using System;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class SnowballGrabZone : HoldableObject
{
	// Token: 0x06000DA8 RID: 3496 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void DropItemCleanup()
	{
	}

	// Token: 0x06000DAA RID: 3498 RVA: 0x0004AF8C File Offset: 0x0004918C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		if (flag ? EquipmentInteractor.instance.disableLeftGrab : EquipmentInteractor.instance.disableRightGrab)
		{
			return;
		}
		SnowballThrowable snowballThrowable;
		(flag ? SnowballMaker.leftHandInstance : SnowballMaker.rightHandInstance).TryCreateSnowball(this.materialIndex, out snowballThrowable);
	}

	// Token: 0x04001052 RID: 4178
	[GorillaSoundLookup]
	public int materialIndex;
}
