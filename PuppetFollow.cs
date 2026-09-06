using System;
using UnityEngine;

// Token: 0x020004F0 RID: 1264
public class PuppetFollow : MonoBehaviour
{
	// Token: 0x06001EB6 RID: 7862 RVA: 0x000A3F90 File Offset: 0x000A2190
	private void FixedUpdate()
	{
		base.transform.position = this.sourceTarget.position - this.sourceBase.position + this.puppetBase.position;
		base.transform.localRotation = this.sourceTarget.localRotation;
	}

	// Token: 0x04002903 RID: 10499
	public Transform sourceTarget;

	// Token: 0x04002904 RID: 10500
	public Transform sourceBase;

	// Token: 0x04002905 RID: 10501
	public Transform puppetBase;
}
