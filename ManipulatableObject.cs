using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000534 RID: 1332
public class ManipulatableObject : HoldableObject
{
	// Token: 0x0600218A RID: 8586 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnStartManipulation(GameObject grabbingHand)
	{
	}

	// Token: 0x0600218B RID: 8587 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
	}

	// Token: 0x0600218C RID: 8588 RVA: 0x00002076 File Offset: 0x00000276
	protected virtual bool ShouldHandDetach(GameObject hand)
	{
		return false;
	}

	// Token: 0x0600218D RID: 8589 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnHeldUpdate(GameObject hand)
	{
	}

	// Token: 0x0600218E RID: 8590 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnReleasedUpdate()
	{
	}

	// Token: 0x0600218F RID: 8591 RVA: 0x000B31D8 File Offset: 0x000B13D8
	public virtual void LateUpdate()
	{
		if (this.isHeld)
		{
			if (this.holdingHand == null)
			{
				EquipmentInteractor.instance.ForceDropManipulatableObject(this);
				return;
			}
			this.OnHeldUpdate(this.holdingHand);
			if (this.ShouldHandDetach(this.holdingHand))
			{
				EquipmentInteractor.instance.ForceDropManipulatableObject(this);
				return;
			}
		}
		else
		{
			this.OnReleasedUpdate();
		}
	}

	// Token: 0x06002190 RID: 8592 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06002191 RID: 8593 RVA: 0x000B3238 File Offset: 0x000B1438
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
		this.isHeld = true;
		this.holdingHand = grabbingHand;
		this.OnStartManipulation(this.holdingHand);
	}

	// Token: 0x06002192 RID: 8594 RVA: 0x000B3280 File Offset: 0x000B1480
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
		Vector3 averageVelocity = GTPlayer.Instance.GetHandVelocityTracker(flag).GetAverageVelocity(true, 0.15f, false);
		if (flag)
		{
			EquipmentInteractor.instance.leftHandHeldEquipment = null;
		}
		else
		{
			EquipmentInteractor.instance.rightHandHeldEquipment = null;
		}
		this.isHeld = false;
		this.holdingHand = null;
		this.OnStopManipulation(releasingHand, averageVelocity);
		return true;
	}

	// Token: 0x06002193 RID: 8595 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void DropItemCleanup()
	{
	}

	// Token: 0x04002C52 RID: 11346
	protected bool isHeld;

	// Token: 0x04002C53 RID: 11347
	protected GameObject holdingHand;
}
