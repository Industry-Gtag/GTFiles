using System;
using UnityEngine;

// Token: 0x020005DF RID: 1503
public class GorillaPlaySpaceForces : MonoBehaviour
{
	// Token: 0x060025AF RID: 9647 RVA: 0x000C872C File Offset: 0x000C692C
	private void Start()
	{
		this.playspaceRigidbody = base.GetComponent<Rigidbody>();
		this.leftHandRigidbody = this.leftHand.GetComponent<Rigidbody>();
		this.leftHandCollider = this.leftHand.GetComponent<Collider>();
		this.rightHandRigidbody = this.rightHand.GetComponent<Rigidbody>();
		this.rightHandCollider = this.rightHand.GetComponent<Collider>();
	}

	// Token: 0x060025B0 RID: 9648 RVA: 0x000C8789 File Offset: 0x000C6989
	private void FixedUpdate()
	{
		if (Time.time >= 0.1f)
		{
			this.bodyCollider.transform.position = this.headsetTransform.position + this.bodyColliderOffset;
		}
	}

	// Token: 0x0400313F RID: 12607
	public GameObject rightHand;

	// Token: 0x04003140 RID: 12608
	public GameObject leftHand;

	// Token: 0x04003141 RID: 12609
	public Collider bodyCollider;

	// Token: 0x04003142 RID: 12610
	private Collider leftHandCollider;

	// Token: 0x04003143 RID: 12611
	private Collider rightHandCollider;

	// Token: 0x04003144 RID: 12612
	public Transform rightHandTransform;

	// Token: 0x04003145 RID: 12613
	public Transform leftHandTransform;

	// Token: 0x04003146 RID: 12614
	private Rigidbody leftHandRigidbody;

	// Token: 0x04003147 RID: 12615
	private Rigidbody rightHandRigidbody;

	// Token: 0x04003148 RID: 12616
	public Vector3 bodyColliderOffset;

	// Token: 0x04003149 RID: 12617
	public float forceConstant;

	// Token: 0x0400314A RID: 12618
	private Vector3 lastLeftHandPosition;

	// Token: 0x0400314B RID: 12619
	private Vector3 lastRightHandPosition;

	// Token: 0x0400314C RID: 12620
	private Rigidbody playspaceRigidbody;

	// Token: 0x0400314D RID: 12621
	public Transform headsetTransform;
}
