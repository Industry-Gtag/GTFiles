using System;
using UnityEngine;

// Token: 0x020005BA RID: 1466
public class DestroyIfNotBeta : MonoBehaviour
{
	// Token: 0x06002521 RID: 9505 RVA: 0x000C7145 File Offset: 0x000C5345
	private void Awake()
	{
		bool shouldKeepIfBeta = this.m_shouldKeepIfBeta;
		bool shouldKeepIfCreatorBuild = this.m_shouldKeepIfCreatorBuild;
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04003098 RID: 12440
	public bool m_shouldKeepIfBeta = true;

	// Token: 0x04003099 RID: 12441
	public bool m_shouldKeepIfCreatorBuild;
}
