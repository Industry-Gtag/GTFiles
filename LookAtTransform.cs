using System;
using UnityEngine;

// Token: 0x02000DB4 RID: 3508
public class LookAtTransform : MonoBehaviour
{
	// Token: 0x0600561A RID: 22042 RVA: 0x001C2423 File Offset: 0x001C0623
	private void Update()
	{
		base.transform.rotation = Quaternion.LookRotation(this.lookAt.position - base.transform.position);
	}

	// Token: 0x0400676A RID: 26474
	[SerializeField]
	private Transform lookAt;
}
