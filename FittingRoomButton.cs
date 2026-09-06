using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000515 RID: 1301
public class FittingRoomButton : GorillaPressableButton
{
	// Token: 0x0600207B RID: 8315 RVA: 0x000AE8FB File Offset: 0x000ACAFB
	public override void Start()
	{
		if (this.currentCosmeticItem.itemName == "")
		{
			this.currentCosmeticItem = CosmeticsController.instance.nullItem;
		}
	}

	// Token: 0x0600207C RID: 8316 RVA: 0x000AE928 File Offset: 0x000ACB28
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

	// Token: 0x0600207D RID: 8317 RVA: 0x000AEA5B File Offset: 0x000ACC5B
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		CosmeticsController.instance.PressFittingRoomButton(this, isLeftHand);
	}

	// Token: 0x0600207E RID: 8318 RVA: 0x000AEA72 File Offset: 0x000ACC72
	public void SetItem(CosmeticsController.CosmeticItem item, bool isInTryOnSet)
	{
		this.currentCosmeticItem = item;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.currentCosmeticItem.itemPicture;
		}
		this.isOn = isInTryOnSet;
		this.UpdateColor();
	}

	// Token: 0x0600207F RID: 8319 RVA: 0x000AEAAC File Offset: 0x000ACCAC
	public void ClearItem()
	{
		if (this.currentCosmeticItem.isNullItem)
		{
			return;
		}
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.blankSprite;
		}
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x04002B51 RID: 11089
	public CosmeticsController.CosmeticItem currentCosmeticItem;

	// Token: 0x04002B52 RID: 11090
	[SerializeField]
	private SpriteRenderer currentCosmeticSprite;

	// Token: 0x04002B53 RID: 11091
	[SerializeField]
	private Sprite blankSprite;

	// Token: 0x04002B54 RID: 11092
	public string noCosmeticText;
}
