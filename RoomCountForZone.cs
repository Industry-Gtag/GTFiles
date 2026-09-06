using System;
using UnityEngine;

// Token: 0x02000D4C RID: 3404
[Serializable]
internal class RoomCountForZone
{
	// Token: 0x17000808 RID: 2056
	// (get) Token: 0x0600545D RID: 21597 RVA: 0x001BC325 File Offset: 0x001BA525
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	// Token: 0x17000809 RID: 2057
	// (get) Token: 0x0600545E RID: 21598 RVA: 0x001BC32D File Offset: 0x001BA52D
	public GTZone Zone
	{
		get
		{
			return this.zone;
		}
	}

	// Token: 0x040065DD RID: 26077
	[SerializeField]
	private GTZone zone;

	// Token: 0x040065DE RID: 26078
	[SerializeField]
	private int count;
}
