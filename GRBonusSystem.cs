using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000762 RID: 1890
public class GRBonusSystem
{
	// Token: 0x06002FEA RID: 12266 RVA: 0x00104924 File Offset: 0x00102B24
	public void Init(GRAttributes attributes)
	{
		this.defaultAttributes = attributes;
	}

	// Token: 0x06002FEB RID: 12267 RVA: 0x0010492D File Offset: 0x00102B2D
	public GRAttributes GetDefaultAttributes()
	{
		return this.defaultAttributes;
	}

	// Token: 0x06002FEC RID: 12268 RVA: 0x00104938 File Offset: 0x00102B38
	public void AddBonus(GRBonusEntry entry)
	{
		if (entry.bonusType == GRBonusEntry.GRBonusType.None)
		{
			return;
		}
		if (!this.currentAdditiveBonuses.ContainsKey(entry.attributeType))
		{
			this.currentAdditiveBonuses[entry.attributeType] = new List<GRBonusEntry>();
		}
		if (!this.currentMultiplicativeBonuses.ContainsKey(entry.attributeType))
		{
			this.currentMultiplicativeBonuses[entry.attributeType] = new List<GRBonusEntry>();
		}
		if (entry.bonusType == GRBonusEntry.GRBonusType.Additive)
		{
			this.currentAdditiveBonuses[entry.attributeType].Add(entry);
			return;
		}
		if (entry.bonusType == GRBonusEntry.GRBonusType.Multiplicative)
		{
			this.currentMultiplicativeBonuses[entry.attributeType].Add(entry);
		}
	}

	// Token: 0x06002FED RID: 12269 RVA: 0x001049E4 File Offset: 0x00102BE4
	public void RemoveBonus(GRBonusEntry entry)
	{
		foreach (List<GRBonusEntry> list in this.currentAdditiveBonuses.Values)
		{
			list.Remove(entry);
		}
		foreach (List<GRBonusEntry> list2 in this.currentMultiplicativeBonuses.Values)
		{
			list2.Remove(entry);
		}
	}

	// Token: 0x06002FEE RID: 12270 RVA: 0x00104A84 File Offset: 0x00102C84
	public bool HasValueForAttribute(GRAttributeType attributeType)
	{
		return this.defaultAttributes != null && this.defaultAttributes.defaultAttributes.ContainsKey(attributeType);
	}

	// Token: 0x06002FEF RID: 12271 RVA: 0x00104AA8 File Offset: 0x00102CA8
	public int CalculateFinalValueForAttribute(GRAttributeType attributeType)
	{
		if (this.defaultAttributes == null)
		{
			Debug.LogErrorFormat("CalculateFinalValueForAttribute DefaultAttributes null.  Please fix configuration.", Array.Empty<object>());
			return 0;
		}
		if (!this.defaultAttributes.defaultAttributes.ContainsKey(attributeType))
		{
			Debug.LogErrorFormat("CalculateFinalValueForAttribute DefaultAttributes Does not have entry for {0}.  Please fix configuration.", new object[] { attributeType });
			return 0;
		}
		int num = this.defaultAttributes.defaultAttributes[attributeType];
		if (this.currentAdditiveBonuses.ContainsKey(attributeType))
		{
			foreach (GRBonusEntry grbonusEntry in this.currentAdditiveBonuses[attributeType])
			{
				if (grbonusEntry.customBonus != null)
				{
					num = grbonusEntry.customBonus(num, grbonusEntry);
				}
				else
				{
					num += grbonusEntry.GetBonusValue();
				}
			}
		}
		if (this.currentMultiplicativeBonuses.ContainsKey(attributeType))
		{
			foreach (GRBonusEntry grbonusEntry2 in this.currentMultiplicativeBonuses[attributeType])
			{
				if (grbonusEntry2.customBonus != null)
				{
					num = grbonusEntry2.customBonus(num, grbonusEntry2);
				}
				else
				{
					float num2 = (float)grbonusEntry2.GetBonusValue() / 100f;
					num = (int)((float)num * num2);
				}
			}
		}
		return num;
	}

	// Token: 0x04003D6E RID: 15726
	private GRAttributes defaultAttributes;

	// Token: 0x04003D6F RID: 15727
	private Dictionary<GRAttributeType, List<GRBonusEntry>> currentAdditiveBonuses = new Dictionary<GRAttributeType, List<GRBonusEntry>>();

	// Token: 0x04003D70 RID: 15728
	private Dictionary<GRAttributeType, List<GRBonusEntry>> currentMultiplicativeBonuses = new Dictionary<GRAttributeType, List<GRBonusEntry>>();
}
