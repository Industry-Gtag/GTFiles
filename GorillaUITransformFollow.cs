using System;
using UnityEngine;

// Token: 0x020008E4 RID: 2276
public class GorillaUITransformFollow : MonoBehaviour
{
	// Token: 0x06003B9B RID: 15259 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Start()
	{
	}

	// Token: 0x06003B9C RID: 15260 RVA: 0x00146A98 File Offset: 0x00144C98
	private void LateUpdate()
	{
		if (this.doesMove)
		{
			base.transform.rotation = this.transformToFollow.rotation;
			base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
		}
	}

	// Token: 0x04004C33 RID: 19507
	public Transform transformToFollow;

	// Token: 0x04004C34 RID: 19508
	public Vector3 offset;

	// Token: 0x04004C35 RID: 19509
	public bool doesMove;
}
