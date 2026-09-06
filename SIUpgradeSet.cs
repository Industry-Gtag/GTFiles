using System;

// Token: 0x02000123 RID: 291
public struct SIUpgradeSet
{
	// Token: 0x0600072E RID: 1838 RVA: 0x0002905E File Offset: 0x0002725E
	public void Clear()
	{
		this.backingBits = 0;
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x00029067 File Offset: 0x00027267
	public SIUpgradeSet(int bits)
	{
		this.backingBits = bits;
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x00029070 File Offset: 0x00027270
	public int GetBits()
	{
		return this.backingBits;
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x00029067 File Offset: 0x00027267
	public void SetBits(int bits)
	{
		this.backingBits = bits;
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x00029078 File Offset: 0x00027278
	public long GetCreateData(SIPlayer player)
	{
		return ((long)this.backingBits << 32) | (long)player.ActorNr;
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x0002908C File Offset: 0x0002728C
	public void Add(SIUpgradeType upgrade)
	{
		this.backingBits |= 1 << upgrade.GetNodeId();
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x000290A6 File Offset: 0x000272A6
	public void Add(int nodeId)
	{
		this.backingBits |= 1 << nodeId;
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x000290BB File Offset: 0x000272BB
	public void Remove(SIUpgradeType upgrade)
	{
		this.backingBits &= ~(1 << upgrade.GetNodeId());
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x000290D6 File Offset: 0x000272D6
	public bool Contains(SIUpgradeType upgrade)
	{
		return (this.backingBits & (1 << upgrade.GetNodeId())) != 0;
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x000290F0 File Offset: 0x000272F0
	public bool ContainsAny(params SIUpgradeType[] upgrades)
	{
		int num = 0;
		foreach (SIUpgradeType siupgradeType in upgrades)
		{
			num |= 1 << siupgradeType.GetNodeId();
		}
		return (this.backingBits & num) != 0;
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x0002912C File Offset: 0x0002732C
	public string GetString(SITechTreePageId pageId)
	{
		string text = "";
		int i = this.backingBits;
		int num = 0;
		bool flag = true;
		while (i > 0)
		{
			if ((i & 1) != 0)
			{
				if (!flag)
				{
					text += "|";
				}
				text += SIUpgradeTypeSystem.GetUpgradeType((int)pageId, num).ToString();
				flag = false;
			}
			i >>= 1;
			num++;
		}
		return text;
	}

	// Token: 0x0400097E RID: 2430
	private int backingBits;
}
