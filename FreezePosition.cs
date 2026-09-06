using System;
using UnityEngine;

// Token: 0x020000A3 RID: 163
public class FreezePosition : MonoBehaviour
{
	// Token: 0x06000405 RID: 1029 RVA: 0x00017EEB File Offset: 0x000160EB
	private void FixedUpdate()
	{
		if (this.target)
		{
			this.target.localPosition = this.localPosition;
		}
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x00017EEB File Offset: 0x000160EB
	private void LateUpdate()
	{
		if (this.target)
		{
			this.target.localPosition = this.localPosition;
		}
	}

	// Token: 0x04000474 RID: 1140
	public Transform target;

	// Token: 0x04000475 RID: 1141
	public Vector3 localPosition;
}
