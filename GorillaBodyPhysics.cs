using System;
using UnityEngine;

// Token: 0x020005D6 RID: 1494
public class GorillaBodyPhysics : MonoBehaviour
{
	// Token: 0x06002594 RID: 9620 RVA: 0x000C836D File Offset: 0x000C656D
	private void FixedUpdate()
	{
		this.bodyCollider.transform.position = this.headsetTransform.position + this.bodyColliderOffset;
	}

	// Token: 0x040030FF RID: 12543
	public GameObject bodyCollider;

	// Token: 0x04003100 RID: 12544
	public Vector3 bodyColliderOffset;

	// Token: 0x04003101 RID: 12545
	public Transform headsetTransform;
}
