using System;
using UnityEngine;

// Token: 0x020005D2 RID: 1490
public class GameModePageButton : GorillaPressableButton
{
	// Token: 0x0600257E RID: 9598 RVA: 0x000C7FFC File Offset: 0x000C61FC
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.selector.ChangePage(this.left);
	}

	// Token: 0x040030EF RID: 12527
	[SerializeField]
	private GameModePages selector;

	// Token: 0x040030F0 RID: 12528
	[SerializeField]
	private bool left;
}
