using System;

// Token: 0x020009FF RID: 2559
public struct HandLinkAuthorityStatus
{
	// Token: 0x0600419D RID: 16797 RVA: 0x0015D823 File Offset: 0x0015BA23
	public HandLinkAuthorityStatus(HandLinkAuthorityType authority)
	{
		this.type = authority;
		this.timestamp = -1f;
		this.tiebreak = -1;
	}

	// Token: 0x0600419E RID: 16798 RVA: 0x0015D83E File Offset: 0x0015BA3E
	public HandLinkAuthorityStatus(HandLinkAuthorityType authority, float timestamp, int tiebreak)
	{
		this.type = authority;
		this.timestamp = timestamp;
		this.tiebreak = tiebreak;
	}

	// Token: 0x0600419F RID: 16799 RVA: 0x0015D858 File Offset: 0x0015BA58
	public static bool operator >(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.type > b.type || (b.type <= a.type && (a.timestamp > b.timestamp || (b.timestamp <= a.timestamp && a.tiebreak > b.tiebreak)));
	}

	// Token: 0x060041A0 RID: 16800 RVA: 0x0015D8B4 File Offset: 0x0015BAB4
	public static bool operator <(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.type < b.type || (b.type >= a.type && (a.timestamp < b.timestamp || (b.timestamp >= a.timestamp && a.tiebreak < b.tiebreak)));
	}

	// Token: 0x060041A1 RID: 16801 RVA: 0x0015D910 File Offset: 0x0015BB10
	public int CompareTo(HandLinkAuthorityStatus b)
	{
		int num = this.type.CompareTo(b.type);
		if (num != 0)
		{
			return num;
		}
		int num2 = this.timestamp.CompareTo(b.timestamp);
		if (num2 != 0)
		{
			return num2;
		}
		return this.tiebreak.CompareTo(b.tiebreak);
	}

	// Token: 0x060041A2 RID: 16802 RVA: 0x0015D967 File Offset: 0x0015BB67
	public static bool operator ==(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.type == b.type && a.timestamp == b.timestamp && a.tiebreak == b.tiebreak;
	}

	// Token: 0x060041A3 RID: 16803 RVA: 0x0015D995 File Offset: 0x0015BB95
	public static bool operator !=(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.timestamp != b.timestamp || a.tiebreak != b.tiebreak;
	}

	// Token: 0x060041A4 RID: 16804 RVA: 0x0015D9B8 File Offset: 0x0015BBB8
	public override string ToString()
	{
		return string.Format("{0}/{1}", this.timestamp.ToString("0.0000"), this.tiebreak);
	}

	// Token: 0x0400524D RID: 21069
	public HandLinkAuthorityType type;

	// Token: 0x0400524E RID: 21070
	public float timestamp;

	// Token: 0x0400524F RID: 21071
	public int tiebreak;
}
