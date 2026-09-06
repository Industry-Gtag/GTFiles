using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004D0 RID: 1232
public class TransferrableObjectHoldablePart : HoldableObject, ITickSystemTick
{
	// Token: 0x17000328 RID: 808
	// (get) Token: 0x06001DF8 RID: 7672 RVA: 0x000A17C7 File Offset: 0x0009F9C7
	// (set) Token: 0x06001DF9 RID: 7673 RVA: 0x000A17CF File Offset: 0x0009F9CF
	public bool TickRunning { get; set; }

	// Token: 0x06001DFA RID: 7674 RVA: 0x0001A297 File Offset: 0x00018497
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06001DFB RID: 7675 RVA: 0x0001A29F File Offset: 0x0001849F
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06001DFC RID: 7676 RVA: 0x000A17D8 File Offset: 0x0009F9D8
	public void Tick()
	{
		VRRig vrrig;
		if (!this.transferrableParentObject.IsLocalObject())
		{
			vrrig = this.transferrableParentObject.myOnlineRig;
			this.isHeld = (this.transferrableParentObject.itemState & this.heldBit) > (TransferrableObject.ItemStates)0;
			TransferrableObject.PositionState currentState = this.transferrableParentObject.currentState;
			if (currentState == TransferrableObject.PositionState.OnRightArm || currentState == TransferrableObject.PositionState.InRightHand)
			{
				this.isHeldLeftHand = this.isHeld;
			}
			else
			{
				this.isHeldLeftHand = false;
			}
		}
		else
		{
			vrrig = VRRig.LocalRig;
		}
		if (this.isHeld)
		{
			if (this.transferrableParentObject.InHand())
			{
				this.UpdateHeld(vrrig, this.isHeldLeftHand);
				return;
			}
			if (this.transferrableParentObject.IsLocalObject())
			{
				this.OnRelease(null, this.isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
			}
		}
	}

	// Token: 0x06001DFD RID: 7677 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void UpdateHeld(VRRig rig, bool isHeldLeftHand)
	{
	}

	// Token: 0x06001DFE RID: 7678 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06001DFF RID: 7679 RVA: 0x000A18A4 File Offset: 0x0009FAA4
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (this.transferrableParentObject.ownerRig && !this.transferrableParentObject.ownerRig.isLocal)
		{
			return;
		}
		this.isHeld = true;
		this.isHeldLeftHand = grabbingHand == EquipmentInteractor.instance.leftHand;
		this.transferrableParentObject.itemState |= this.heldBit;
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		UnityEvent unityEvent = this.onGrab;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06001E00 RID: 7680 RVA: 0x000A1930 File Offset: 0x0009FB30
	public override void DropItemCleanup()
	{
		this.isHeld = false;
		this.isHeldLeftHand = false;
		this.transferrableParentObject.itemState &= ~this.heldBit;
	}

	// Token: 0x06001E01 RID: 7681 RVA: 0x000A195C File Offset: 0x0009FB5C
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
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
		this.isHeld = false;
		this.isHeldLeftHand = false;
		this.transferrableParentObject.itemState &= ~this.heldBit;
		UnityEvent unityEvent = this.onRelease;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		return true;
	}

	// Token: 0x04002853 RID: 10323
	[SerializeField]
	protected TransferrableObject transferrableParentObject;

	// Token: 0x04002854 RID: 10324
	[SerializeField]
	private TransferrableObject.ItemStates heldBit = TransferrableObject.ItemStates.Part0Held;

	// Token: 0x04002855 RID: 10325
	private bool isHeld;

	// Token: 0x04002856 RID: 10326
	protected bool isHeldLeftHand;

	// Token: 0x04002857 RID: 10327
	public UnityEvent onGrab;

	// Token: 0x04002858 RID: 10328
	public UnityEvent onRelease;

	// Token: 0x04002859 RID: 10329
	public UnityEvent onDrop;
}
