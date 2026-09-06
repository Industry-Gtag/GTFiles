using System;
using UnityEngine;

// Token: 0x020002E6 RID: 742
public class IgnoreLocalRotation : MonoBehaviour
{
	// Token: 0x060012E2 RID: 4834 RVA: 0x00064A13 File Offset: 0x00062C13
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.identity;
	}
}
