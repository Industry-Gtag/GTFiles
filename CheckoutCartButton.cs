using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000505 RID: 1285
public class CheckoutCartButton : GorillaPressableButton
{
	// Token: 0x0600203E RID: 8254 RVA: 0x000AD7F7 File Offset: 0x000AB9F7
	public override void Start()
	{
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
	}

	// Token: 0x0600203F RID: 8255 RVA: 0x000AD80C File Offset: 0x000ABA0C
	public override void UpdateColor()
	{
		if (this.currentCosmeticItem.itemName == "null")
		{
			if (this.buttonRenderer.IsNotNull())
			{
				this.buttonRenderer.material = this.unpressedMaterial;
			}
			if (this.myText.IsNotNull())
			{
				this.myText.text = this.noCosmeticText;
			}
			if (this.myTmpText.IsNotNull())
			{
				this.myTmpText.text = this.noCosmeticText;
			}
			if (this.myTmpText2.IsNotNull())
			{
				this.myTmpText2.text = this.noCosmeticText;
				return;
			}
		}
		else
		{
			if (this.isOn)
			{
				if (this.buttonRenderer.IsNotNull())
				{
					this.buttonRenderer.material = this.pressedMaterial;
				}
				this.SetOnText(this.myText.IsNotNull(), this.myTmpText.IsNotNull(), this.myTmpText2.IsNotNull());
				return;
			}
			if (this.buttonRenderer.IsNotNull())
			{
				this.buttonRenderer.material = this.unpressedMaterial;
			}
			this.SetOffText(this.myText.IsNotNull(), this.myTmpText.IsNotNull(), this.myTmpText2.IsNotNull());
		}
	}

	// Token: 0x06002040 RID: 8256 RVA: 0x000AD93F File Offset: 0x000ABB3F
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCheckoutCartButton(this, isLeftHand);
	}

	// Token: 0x06002041 RID: 8257 RVA: 0x000AD955 File Offset: 0x000ABB55
	public void SetItem(CosmeticsController.CosmeticItem item, bool isCurrentItemToBuy)
	{
		this.currentCosmeticItem = item;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.currentCosmeticItem.itemPicture;
		}
		this.isOn = isCurrentItemToBuy;
		this.UpdateColor();
	}

	// Token: 0x06002042 RID: 8258 RVA: 0x000AD98E File Offset: 0x000ABB8E
	public void ClearItem()
	{
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.blankSprite;
		}
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x04002B0C RID: 11020
	public CosmeticsController.CosmeticItem currentCosmeticItem;

	// Token: 0x04002B0D RID: 11021
	[SerializeField]
	private SpriteRenderer currentCosmeticSprite;

	// Token: 0x04002B0E RID: 11022
	[SerializeField]
	private Sprite blankSprite;

	// Token: 0x04002B0F RID: 11023
	public string noCosmeticText;
}
