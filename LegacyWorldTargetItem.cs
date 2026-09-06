using System;
using Photon.Realtime;

// Token: 0x020004B7 RID: 1207
public class LegacyWorldTargetItem
{
	// Token: 0x06001D80 RID: 7552 RVA: 0x0009F6A8 File Offset: 0x0009D8A8
	public bool IsValid()
	{
		return this.itemIdx != -1 && this.owner != null;
	}

	// Token: 0x06001D81 RID: 7553 RVA: 0x0009F6BE File Offset: 0x0009D8BE
	public void Invalidate()
	{
		this.itemIdx = -1;
		this.owner = null;
	}

	// Token: 0x040027BC RID: 10172
	public Player owner;

	// Token: 0x040027BD RID: 10173
	public int itemIdx;
}
