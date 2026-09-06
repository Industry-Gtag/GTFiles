using System;
using UnityEngine;

// Token: 0x02000C14 RID: 3092
public class LockRotation : MonoBehaviour
{
	// Token: 0x06004D62 RID: 19810 RVA: 0x0019C879 File Offset: 0x0019AA79
	private void Start()
	{
		this.lockedRot = base.transform.rotation;
	}

	// Token: 0x06004D63 RID: 19811 RVA: 0x0019C88C File Offset: 0x0019AA8C
	private void LateUpdate()
	{
		base.transform.rotation = this.lockedRot;
	}

	// Token: 0x040060BC RID: 24764
	private Quaternion lockedRot;
}
