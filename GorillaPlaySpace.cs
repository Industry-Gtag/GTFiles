using System;
using UnityEngine;

// Token: 0x020005DE RID: 1502
public class GorillaPlaySpace : MonoBehaviour
{
	// Token: 0x170003F9 RID: 1017
	// (get) Token: 0x060025AC RID: 9644 RVA: 0x000C8668 File Offset: 0x000C6868
	public static GorillaPlaySpace Instance
	{
		get
		{
			return GorillaPlaySpace._instance;
		}
	}

	// Token: 0x060025AD RID: 9645 RVA: 0x000C866F File Offset: 0x000C686F
	private void Awake()
	{
		if (GorillaPlaySpace._instance != null && GorillaPlaySpace._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		GorillaPlaySpace._instance = this;
	}

	// Token: 0x0400311B RID: 12571
	[OnEnterPlay_SetNull]
	private static GorillaPlaySpace _instance;

	// Token: 0x0400311C RID: 12572
	public Collider headCollider;

	// Token: 0x0400311D RID: 12573
	public Collider bodyCollider;

	// Token: 0x0400311E RID: 12574
	public Transform rightHandTransform;

	// Token: 0x0400311F RID: 12575
	public Transform leftHandTransform;

	// Token: 0x04003120 RID: 12576
	public Vector3 headColliderOffset;

	// Token: 0x04003121 RID: 12577
	public Vector3 bodyColliderOffset;

	// Token: 0x04003122 RID: 12578
	private Vector3 lastLeftHandPosition;

	// Token: 0x04003123 RID: 12579
	private Vector3 lastRightHandPosition;

	// Token: 0x04003124 RID: 12580
	private Vector3 lastLeftHandPositionForTag;

	// Token: 0x04003125 RID: 12581
	private Vector3 lastRightHandPositionForTag;

	// Token: 0x04003126 RID: 12582
	private Vector3 lastBodyPositionForTag;

	// Token: 0x04003127 RID: 12583
	private Vector3 lastHeadPositionForTag;

	// Token: 0x04003128 RID: 12584
	private Rigidbody playspaceRigidbody;

	// Token: 0x04003129 RID: 12585
	public Transform headsetTransform;

	// Token: 0x0400312A RID: 12586
	public Vector3 rightHandOffset;

	// Token: 0x0400312B RID: 12587
	public Vector3 leftHandOffset;

	// Token: 0x0400312C RID: 12588
	public VRRig vrRig;

	// Token: 0x0400312D RID: 12589
	public VRRig offlineVRRig;

	// Token: 0x0400312E RID: 12590
	public float vibrationCooldown = 0.1f;

	// Token: 0x0400312F RID: 12591
	public float vibrationDuration = 0.05f;

	// Token: 0x04003130 RID: 12592
	private float leftLastTouchedSurface;

	// Token: 0x04003131 RID: 12593
	private float rightLastTouchedSurface;

	// Token: 0x04003132 RID: 12594
	public VRRig myVRRig;

	// Token: 0x04003133 RID: 12595
	private float bodyHeight;

	// Token: 0x04003134 RID: 12596
	public float tagCooldown;

	// Token: 0x04003135 RID: 12597
	public float taggedTime;

	// Token: 0x04003136 RID: 12598
	public float disconnectTime = 60f;

	// Token: 0x04003137 RID: 12599
	public float maxStepVelocity = 2f;

	// Token: 0x04003138 RID: 12600
	public float hapticWaitSeconds = 0.05f;

	// Token: 0x04003139 RID: 12601
	public float tapHapticDuration = 0.05f;

	// Token: 0x0400313A RID: 12602
	public float tapHapticStrength = 0.5f;

	// Token: 0x0400313B RID: 12603
	public float tagHapticDuration = 0.15f;

	// Token: 0x0400313C RID: 12604
	public float tagHapticStrength = 1f;

	// Token: 0x0400313D RID: 12605
	public float taggedHapticDuration = 0.35f;

	// Token: 0x0400313E RID: 12606
	public float taggedHapticStrength = 1f;
}
