using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200053A RID: 1338
[NetworkBehaviourWeaved(0)]
public abstract class NetworkHoldableObject : NetworkComponent, IHoldableObject
{
	// Token: 0x1700039B RID: 923
	// (get) Token: 0x060021BF RID: 8639 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool TwoHanded
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060021C0 RID: 8640
	public abstract void OnHover(InteractionPoint pointHovered, GameObject hoveringHand);

	// Token: 0x060021C1 RID: 8641
	public abstract void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand);

	// Token: 0x060021C2 RID: 8642
	public abstract void DropItemCleanup();

	// Token: 0x060021C3 RID: 8643 RVA: 0x000B412C File Offset: 0x000B232C
	public virtual bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (EquipmentInteractor.instance.rightHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (EquipmentInteractor.instance.leftHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	// Token: 0x060021C4 RID: 8644 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void ReadDataFusion()
	{
	}

	// Token: 0x060021C5 RID: 8645 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void WriteDataFusion()
	{
	}

	// Token: 0x060021C6 RID: 8646 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060021C7 RID: 8647 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060021C9 RID: 8649 RVA: 0x000066D3 File Offset: 0x000048D3
	GameObject IHoldableObject.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x060021CA RID: 8650 RVA: 0x00014B5B File Offset: 0x00012D5B
	string IHoldableObject.get_name()
	{
		return base.name;
	}

	// Token: 0x060021CB RID: 8651 RVA: 0x00014B63 File Offset: 0x00012D63
	void IHoldableObject.set_name(string value)
	{
		base.name = value;
	}

	// Token: 0x060021CC RID: 8652 RVA: 0x00002E6F File Offset: 0x0000106F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060021CD RID: 8653 RVA: 0x00002E7B File Offset: 0x0000107B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
