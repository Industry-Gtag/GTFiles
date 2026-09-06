using System;
using UnityEngine;

// Token: 0x02000A3F RID: 2623
public class GorillaTriggerColliderHandIndicator : MonoBehaviourTick
{
	// Token: 0x0600435B RID: 17243 RVA: 0x00166834 File Offset: 0x00164A34
	public override void Tick()
	{
		this.currentVelocity = (base.transform.position - this.lastPosition) / Time.deltaTime;
		this.lastPosition = base.transform.position;
	}

	// Token: 0x0600435C RID: 17244 RVA: 0x0016686D File Offset: 0x00164A6D
	private void OnTriggerEnter(Collider other)
	{
		if (this.throwableController != null)
		{
			this.throwableController.GrabbableObjectHover(this.isLeftHand);
		}
	}

	// Token: 0x0400553E RID: 21822
	public Vector3 currentVelocity;

	// Token: 0x0400553F RID: 21823
	public Vector3 lastPosition = Vector3.zero;

	// Token: 0x04005540 RID: 21824
	public bool isLeftHand;

	// Token: 0x04005541 RID: 21825
	public GorillaThrowableController throwableController;
}
