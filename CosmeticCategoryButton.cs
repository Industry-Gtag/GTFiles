using System;
using UnityEngine;

// Token: 0x0200057D RID: 1405
public class CosmeticCategoryButton : CosmeticButton
{
	// Token: 0x060023B8 RID: 9144 RVA: 0x000C0228 File Offset: 0x000BE428
	public void SetIcon(Sprite sprite)
	{
		this.equippedLeftIcon.enabled = false;
		this.equippedRightIcon.enabled = false;
		this.equippedIcon.enabled = sprite != null;
		this.equippedIcon.sprite = sprite;
	}

	// Token: 0x060023B9 RID: 9145 RVA: 0x000C0260 File Offset: 0x000BE460
	public void SetDualIcon(Sprite leftSprite, Sprite rightSprite)
	{
		this.equippedLeftIcon.enabled = leftSprite != null;
		this.equippedRightIcon.enabled = rightSprite != null;
		this.equippedIcon.enabled = false;
		this.equippedLeftIcon.sprite = leftSprite;
		this.equippedRightIcon.sprite = rightSprite;
	}

	// Token: 0x060023BA RID: 9146 RVA: 0x000C02B8 File Offset: 0x000BE4B8
	public override void UpdatePosition()
	{
		base.UpdatePosition();
		if (this.equippedIcon != null)
		{
			this.equippedIcon.transform.position += this.posOffset;
		}
		if (this.equippedLeftIcon != null)
		{
			this.equippedLeftIcon.transform.position += this.posOffset;
		}
		if (this.equippedRightIcon != null)
		{
			this.equippedRightIcon.transform.position += this.posOffset;
		}
	}

	// Token: 0x04002EEA RID: 12010
	[SerializeField]
	private SpriteRenderer equippedIcon;

	// Token: 0x04002EEB RID: 12011
	[SerializeField]
	private SpriteRenderer equippedLeftIcon;

	// Token: 0x04002EEC RID: 12012
	[SerializeField]
	private SpriteRenderer equippedRightIcon;
}
