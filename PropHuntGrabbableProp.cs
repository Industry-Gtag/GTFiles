using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200027A RID: 634
public class PropHuntGrabbableProp : HoldableObject
{
	// Token: 0x06001121 RID: 4385 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06001122 RID: 4386 RVA: 0x0005B9E4 File Offset: 0x00059BE4
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		this.handFollower.SwitchHand(flag);
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
	}

	// Token: 0x06001123 RID: 4387 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void DropItemCleanup()
	{
	}

	// Token: 0x06001124 RID: 4388 RVA: 0x0005BA20 File Offset: 0x00059C20
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (EquipmentInteractor.instance.rightHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (EquipmentInteractor.instance.leftHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	// Token: 0x0400145A RID: 5210
	public PropHuntHandFollower handFollower;

	// Token: 0x0400145B RID: 5211
	public Vector3 offset;

	// Token: 0x0400145C RID: 5212
	public List<InteractionPoint> interactionPoints;
}
