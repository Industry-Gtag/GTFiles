using System;

// Token: 0x02000122 RID: 290
public static class SIUpgradeTypeSystem
{
	// Token: 0x0600072B RID: 1835 RVA: 0x0002904A File Offset: 0x0002724A
	public static int GetPageId(this SIUpgradeType self)
	{
		return (int)(self / SIUpgradeType.Stilt_Unlock);
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x00029050 File Offset: 0x00027250
	public static int GetNodeId(this SIUpgradeType self)
	{
		return (int)(self % SIUpgradeType.Stilt_Unlock);
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x00029056 File Offset: 0x00027256
	public static SIUpgradeType GetUpgradeType(int pageId, int nodeId)
	{
		return (SIUpgradeType)(pageId * 100 + nodeId);
	}
}
