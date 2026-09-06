using System;
using UnityEngine;

// Token: 0x0200052E RID: 1326
public abstract class HoldableObject : MonoBehaviour, IHoldableObject
{
	// Token: 0x17000396 RID: 918
	// (get) Token: 0x06002142 RID: 8514 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool TwoHanded
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002143 RID: 8515 RVA: 0x000B1F13 File Offset: 0x000B0113
	protected void OnDestroy()
	{
		if (EquipmentInteractor.hasInstance)
		{
			EquipmentInteractor.instance.ForceDropEquipment(this);
		}
	}

	// Token: 0x06002144 RID: 8516
	public abstract void OnHover(InteractionPoint pointHovered, GameObject hoveringHand);

	// Token: 0x06002145 RID: 8517
	public abstract void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand);

	// Token: 0x06002146 RID: 8518
	public abstract void DropItemCleanup();

	// Token: 0x06002147 RID: 8519 RVA: 0x000B1F2C File Offset: 0x000B012C
	public virtual bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (EquipmentInteractor.instance.rightHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (EquipmentInteractor.instance.leftHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	// Token: 0x06002149 RID: 8521 RVA: 0x000066D3 File Offset: 0x000048D3
	GameObject IHoldableObject.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0600214A RID: 8522 RVA: 0x00014B5B File Offset: 0x00012D5B
	string IHoldableObject.get_name()
	{
		return base.name;
	}

	// Token: 0x0600214B RID: 8523 RVA: 0x00014B63 File Offset: 0x00012D63
	void IHoldableObject.set_name(string value)
	{
		base.name = value;
	}
}
