using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200050E RID: 1294
public class CosmeticStand : GorillaPressableButton
{
	// Token: 0x06002056 RID: 8278 RVA: 0x000ADF18 File Offset: 0x000AC118
	public void InitializeCosmetic()
	{
		this.thisCosmeticItem = CosmeticsController.instance.allCosmetics.Find((CosmeticsController.CosmeticItem x) => this.thisCosmeticName == x.displayName || this.thisCosmeticName == x.overrideDisplayName || this.thisCosmeticName == x.itemName);
		if (this.slotPriceText != null)
		{
			this.slotPriceText.text = this.thisCosmeticItem.itemCategory.ToString().ToUpper() + " " + this.thisCosmeticItem.cost.ToString();
		}
	}

	// Token: 0x06002057 RID: 8279 RVA: 0x000ADF96 File Offset: 0x000AC196
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCosmeticStandButton(this);
	}

	// Token: 0x04002B2E RID: 11054
	public CosmeticsController.CosmeticItem thisCosmeticItem;

	// Token: 0x04002B2F RID: 11055
	public string thisCosmeticName;

	// Token: 0x04002B30 RID: 11056
	public HeadModel thisHeadModel;

	// Token: 0x04002B31 RID: 11057
	public Text slotPriceText;

	// Token: 0x04002B32 RID: 11058
	public Text addToCartText;

	// Token: 0x04002B33 RID: 11059
	[Tooltip("If this is true then this cosmetic stand should have already been updated when the 'Update Cosmetic Stands' button was pressed in the CosmeticsController inspector.")]
	public bool skipMe;
}
