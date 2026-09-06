using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000757 RID: 1879
public class GRAttributes : MonoBehaviour
{
	// Token: 0x06002FB5 RID: 12213 RVA: 0x00103C9C File Offset: 0x00101E9C
	private void Awake()
	{
		foreach (GRAttributes.GRAttributePair grattributePair in this.startingAttributes)
		{
			this.defaultAttributes[grattributePair.type] = (int)(grattributePair.value * 100f);
		}
		this.bonusSystem.Init(this);
	}

	// Token: 0x06002FB6 RID: 12214 RVA: 0x00103D14 File Offset: 0x00101F14
	public bool HasBeenInitialized()
	{
		return this.bonusSystem.GetDefaultAttributes() != null;
	}

	// Token: 0x06002FB7 RID: 12215 RVA: 0x00103D27 File Offset: 0x00101F27
	public void AddAttribute(GRAttributeType type, float value)
	{
		this.defaultAttributes[type] = (int)(value * 100f);
	}

	// Token: 0x06002FB8 RID: 12216 RVA: 0x00103D3D File Offset: 0x00101F3D
	public void AddBonus(GRBonusEntry entry)
	{
		this.bonusSystem.AddBonus(entry);
	}

	// Token: 0x06002FB9 RID: 12217 RVA: 0x00103D4B File Offset: 0x00101F4B
	public void RemoveBonus(GRBonusEntry entry)
	{
		this.bonusSystem.RemoveBonus(entry);
	}

	// Token: 0x06002FBA RID: 12218 RVA: 0x00103D5C File Offset: 0x00101F5C
	public float CalculateFinalFloatValueForAttribute(GRAttributeType attributeType)
	{
		int num = this.bonusSystem.CalculateFinalValueForAttribute(attributeType);
		float num2 = 0f;
		if (num > 0)
		{
			num2 = (float)num / 100f;
		}
		return num2;
	}

	// Token: 0x06002FBB RID: 12219 RVA: 0x00103D8C File Offset: 0x00101F8C
	public int CalculateFinalValueForAttribute(GRAttributeType attributeType)
	{
		int num = this.bonusSystem.CalculateFinalValueForAttribute(attributeType);
		if (num > 0)
		{
			num /= 100;
		}
		return num;
	}

	// Token: 0x06002FBC RID: 12220 RVA: 0x00103DB0 File Offset: 0x00101FB0
	public bool HasValueForAttribute(GRAttributeType attributeType)
	{
		return this.bonusSystem.HasValueForAttribute(attributeType);
	}

	// Token: 0x04003D27 RID: 15655
	[SerializeField]
	private List<GRAttributes.GRAttributePair> startingAttributes;

	// Token: 0x04003D28 RID: 15656
	[NonSerialized]
	private GRBonusSystem bonusSystem = new GRBonusSystem();

	// Token: 0x04003D29 RID: 15657
	public Dictionary<GRAttributeType, int> defaultAttributes = new Dictionary<GRAttributeType, int>();

	// Token: 0x02000758 RID: 1880
	[Serializable]
	public struct GRAttributePair
	{
		// Token: 0x04003D2A RID: 15658
		public GRAttributeType type;

		// Token: 0x04003D2B RID: 15659
		public float value;
	}
}
