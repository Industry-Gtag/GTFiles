using System;
using UnityEngine;

// Token: 0x020005E9 RID: 1513
public class GorillaWalkingGrab : MonoBehaviour
{
	// Token: 0x060025C6 RID: 9670 RVA: 0x000C8941 File Offset: 0x000C6B41
	private void Start()
	{
		this.thisRigidbody = base.gameObject.GetComponent<Rigidbody>();
		this.positionHistory = new Vector3[this.historySteps];
		this.historyIndex = 0;
	}

	// Token: 0x060025C7 RID: 9671 RVA: 0x000C896C File Offset: 0x000C6B6C
	private void FixedUpdate()
	{
		this.historyIndex++;
		if (this.historyIndex >= this.historySteps)
		{
			this.historyIndex = 0;
		}
		this.positionHistory[this.historyIndex] = this.handToStickTo.transform.position;
		this.thisRigidbody.MovePosition(this.handToStickTo.transform.position);
		base.transform.rotation = this.handToStickTo.transform.rotation;
	}

	// Token: 0x060025C8 RID: 9672 RVA: 0x00002076 File Offset: 0x00000276
	private bool MakeJump()
	{
		return false;
	}

	// Token: 0x060025C9 RID: 9673 RVA: 0x000C89F4 File Offset: 0x000C6BF4
	private void OnCollisionStay(Collision collision)
	{
		if (!this.MakeJump())
		{
			Vector3 vector = Vector3.ProjectOnPlane(this.positionHistory[(this.historyIndex != 0) ? (this.historyIndex - 1) : (this.historySteps - 1)] - this.handToStickTo.transform.position, collision.GetContact(0).normal);
			Vector3 vector2 = this.thisRigidbody.transform.position - this.handToStickTo.transform.position;
			this.playspaceRigidbody.MovePosition(this.playspaceRigidbody.transform.position + vector - vector2);
		}
	}

	// Token: 0x04003160 RID: 12640
	public GameObject handToStickTo;

	// Token: 0x04003161 RID: 12641
	public float ratioToUse;

	// Token: 0x04003162 RID: 12642
	public float forceMultiplier;

	// Token: 0x04003163 RID: 12643
	public int historySteps;

	// Token: 0x04003164 RID: 12644
	public Rigidbody playspaceRigidbody;

	// Token: 0x04003165 RID: 12645
	private Rigidbody thisRigidbody;

	// Token: 0x04003166 RID: 12646
	private Vector3 lastPosition;

	// Token: 0x04003167 RID: 12647
	private Vector3 maybeLastPositionIDK;

	// Token: 0x04003168 RID: 12648
	private Vector3[] positionHistory;

	// Token: 0x04003169 RID: 12649
	private int historyIndex;
}
