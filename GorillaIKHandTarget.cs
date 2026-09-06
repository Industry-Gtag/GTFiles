using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020005DD RID: 1501
public class GorillaIKHandTarget : MonoBehaviour
{
	// Token: 0x060025A8 RID: 9640 RVA: 0x000C861D File Offset: 0x000C681D
	private void Start()
	{
		this.thisRigidbody = base.gameObject.GetComponent<Rigidbody>();
	}

	// Token: 0x060025A9 RID: 9641 RVA: 0x000C8630 File Offset: 0x000C6830
	private void FixedUpdate()
	{
		this.thisRigidbody.MovePosition(this.handToStickTo.transform.position);
		base.transform.rotation = this.handToStickTo.transform.rotation;
	}

	// Token: 0x060025AA RID: 9642 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x04003116 RID: 12566
	public GameObject handToStickTo;

	// Token: 0x04003117 RID: 12567
	public bool isLeftHand;

	// Token: 0x04003118 RID: 12568
	public float hapticStrength;

	// Token: 0x04003119 RID: 12569
	private Rigidbody thisRigidbody;

	// Token: 0x0400311A RID: 12570
	private XRController controllerReference;
}
