using System;
using UnityEngine;

// Token: 0x02000AFF RID: 2815
public static class Id128Ext
{
	// Token: 0x0600481C RID: 18460 RVA: 0x00184BC8 File Offset: 0x00182DC8
	public static Id128 ToId128(this Hash128 h)
	{
		return new Id128(h);
	}

	// Token: 0x0600481D RID: 18461 RVA: 0x00184BC0 File Offset: 0x00182DC0
	public static Id128 ToId128(this Guid g)
	{
		return new Id128(g);
	}
}
