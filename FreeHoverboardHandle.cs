using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000902 RID: 2306
public class FreeHoverboardHandle : HoldableObject
{
	// Token: 0x06003C77 RID: 15479 RVA: 0x0014A1C0 File Offset: 0x001483C0
	private void Awake()
	{
		this.hasParentBoard = this.parentFreeBoard != null;
	}

	// Token: 0x06003C78 RID: 15480 RVA: 0x0014A1D4 File Offset: 0x001483D4
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

	// Token: 0x06003C79 RID: 15481 RVA: 0x0014A244 File Offset: 0x00148444
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		if (this.hasParentBoard)
		{
			FreeHoverboardManager.instance.SendGrabBoardRPC(this.parentFreeBoard);
			Transform transform = (flag ? VRRig.LocalRig.leftHand.rigTarget : VRRig.LocalRig.rightHand.rigTarget);
			Quaternion quaternion = transform.InverseTransformRotation(base.transform.rotation);
			Vector3 vector = transform.InverseTransformPoint(base.transform.position);
			GTPlayer.Instance.GrabPersonalHoverboard(flag, vector, quaternion, this.parentFreeBoard.boardColor);
			return;
		}
		Quaternion quaternion2 = (flag ? this.defaultHoldAngleLeft : this.defaultHoldAngleRight);
		Vector3 vector2 = (flag ? this.defaultHoldPosLeft : this.defaultHoldPosRight);
		GTPlayer.Instance.GrabPersonalHoverboard(flag, vector2, quaternion2, VRRig.LocalRig.playerColor);
	}

	// Token: 0x06003C7A RID: 15482 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void DropItemCleanup()
	{
	}

	// Token: 0x06003C7B RID: 15483 RVA: 0x00002E60 File Offset: 0x00001060
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		throw new NotImplementedException();
	}

	// Token: 0x04004D2E RID: 19758
	[SerializeField]
	private FreeHoverboardInstance parentFreeBoard;

	// Token: 0x04004D2F RID: 19759
	private bool hasParentBoard;

	// Token: 0x04004D30 RID: 19760
	[SerializeField]
	private Vector3 defaultHoldPosLeft;

	// Token: 0x04004D31 RID: 19761
	[SerializeField]
	private Vector3 defaultHoldPosRight;

	// Token: 0x04004D32 RID: 19762
	[SerializeField]
	private Quaternion defaultHoldAngleLeft;

	// Token: 0x04004D33 RID: 19763
	[SerializeField]
	private Quaternion defaultHoldAngleRight;

	// Token: 0x04004D34 RID: 19764
	private int noHapticsUntilFrame = -1;
}
