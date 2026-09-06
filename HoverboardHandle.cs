using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000909 RID: 2313
public class HoverboardHandle : HoldableObject
{
	// Token: 0x06003CA2 RID: 15522 RVA: 0x0014AD44 File Offset: 0x00148F44
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		if (Time.frameCount > this.noHapticsUntilFrame)
		{
			GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		this.noHapticsUntilFrame = Time.frameCount + 1;
	}

	// Token: 0x06003CA3 RID: 15523 RVA: 0x0014ADB4 File Offset: 0x00148FB4
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		Transform transform = (flag ? VRRig.LocalRig.leftHand.rigTarget : VRRig.LocalRig.rightHand.rigTarget);
		Quaternion quaternion;
		Vector3 vector;
		if (!this.parentVisual.IsHeld)
		{
			quaternion = (flag ? this.defaultHoldAngleLeft : this.defaultHoldAngleRight);
			vector = (flag ? this.defaultHoldPosLeft : this.defaultHoldPosRight);
		}
		else
		{
			quaternion = transform.InverseTransformRotation(this.parentVisual.transform.rotation);
			vector = transform.InverseTransformPoint(this.parentVisual.transform.position);
		}
		this.parentVisual.SetIsHeld(flag, vector, quaternion, this.parentVisual.boardColor);
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
	}

	// Token: 0x06003CA4 RID: 15524 RVA: 0x0014AE8D File Offset: 0x0014908D
	public override void DropItemCleanup()
	{
		if (this.parentVisual.gameObject.activeSelf)
		{
			this.parentVisual.DropFreeBoard();
		}
		this.parentVisual.SetNotHeld();
	}

	// Token: 0x06003CA5 RID: 15525 RVA: 0x0014AEB8 File Offset: 0x001490B8
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.rightHand)
		{
			return false;
		}
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.leftHand)
		{
			return false;
		}
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.parentVisual.IsLeftHanded);
		this.parentVisual.SetNotHeld();
		return true;
	}

	// Token: 0x04004D59 RID: 19801
	[SerializeField]
	private HoverboardVisual parentVisual;

	// Token: 0x04004D5A RID: 19802
	[SerializeField]
	private Quaternion defaultHoldAngleLeft;

	// Token: 0x04004D5B RID: 19803
	[SerializeField]
	private Quaternion defaultHoldAngleRight;

	// Token: 0x04004D5C RID: 19804
	[SerializeField]
	private Vector3 defaultHoldPosLeft;

	// Token: 0x04004D5D RID: 19805
	[SerializeField]
	private Vector3 defaultHoldPosRight;

	// Token: 0x04004D5E RID: 19806
	private int noHapticsUntilFrame = -1;
}
