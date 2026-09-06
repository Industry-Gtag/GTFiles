using System;
using UnityEngine.UI;

// Token: 0x02000637 RID: 1591
public class BuilderKioskButton : GorillaPressableButton
{
	// Token: 0x060027B3 RID: 10163 RVA: 0x000D2BE2 File Offset: 0x000D0DE2
	public override void Start()
	{
		this.currentPieceSet = BuilderKiosk.nullItem;
	}

	// Token: 0x060027B4 RID: 10164 RVA: 0x000D2BEF File Offset: 0x000D0DEF
	public override void UpdateColor()
	{
		if (this.currentPieceSet.isNullItem)
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			this.myText.text = "";
			return;
		}
		base.UpdateColor();
	}

	// Token: 0x060027B5 RID: 10165 RVA: 0x000D2C26 File Offset: 0x000D0E26
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
	}

	// Token: 0x04003372 RID: 13170
	public BuilderSetManager.BuilderSetStoreItem currentPieceSet;

	// Token: 0x04003373 RID: 13171
	public BuilderKiosk kiosk;

	// Token: 0x04003374 RID: 13172
	public Text setNameText;
}
