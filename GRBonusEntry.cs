using System;
using UnityEngine;

// Token: 0x02000760 RID: 1888
[Serializable]
public class GRBonusEntry
{
	// Token: 0x06002FE5 RID: 12261 RVA: 0x0010487A File Offset: 0x00102A7A
	private GRBonusEntry()
	{
		GRBonusEntry.idCounter++;
		this.id = GRBonusEntry.idCounter;
	}

	// Token: 0x17000482 RID: 1154
	// (get) Token: 0x06002FE6 RID: 12262 RVA: 0x00104899 File Offset: 0x00102A99
	// (set) Token: 0x06002FE7 RID: 12263 RVA: 0x001048A1 File Offset: 0x00102AA1
	public int id { get; private set; }

	// Token: 0x06002FE8 RID: 12264 RVA: 0x001048AA File Offset: 0x00102AAA
	public int GetBonusValue()
	{
		return (int)(this.bonusValue * 100f);
	}

	// Token: 0x06002FE9 RID: 12265 RVA: 0x001048BC File Offset: 0x00102ABC
	public override string ToString()
	{
		bool flag = this.customBonus != null;
		return string.Format("GRBonusEntry BonusType {0} AttributeType {1} BonusValue {2} Id {3} CustomBonusSet {4}", new object[] { this.bonusType, this.attributeType, this.bonusValue, this.id, flag });
	}

	// Token: 0x04003D64 RID: 15716
	private static int idCounter;

	// Token: 0x04003D65 RID: 15717
	public GRBonusEntry.GRBonusType bonusType;

	// Token: 0x04003D66 RID: 15718
	public GRAttributeType attributeType;

	// Token: 0x04003D67 RID: 15719
	[SerializeField]
	private float bonusValue;

	// Token: 0x04003D69 RID: 15721
	public Func<int, GRBonusEntry, int> customBonus;

	// Token: 0x02000761 RID: 1889
	public enum GRBonusType
	{
		// Token: 0x04003D6B RID: 15723
		None,
		// Token: 0x04003D6C RID: 15724
		Additive,
		// Token: 0x04003D6D RID: 15725
		Multiplicative
	}
}
