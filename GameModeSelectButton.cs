using System;
using UnityEngine;

// Token: 0x020005D4 RID: 1492
public class GameModeSelectButton : GorillaPressableButton
{
	// Token: 0x0600258D RID: 9613 RVA: 0x000C82D3 File Offset: 0x000C64D3
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.selector.SelectEntryOnPage(this.buttonIndex);
	}

	// Token: 0x040030F8 RID: 12536
	[SerializeField]
	internal GameModePages selector;

	// Token: 0x040030F9 RID: 12537
	[SerializeField]
	internal int buttonIndex;
}
