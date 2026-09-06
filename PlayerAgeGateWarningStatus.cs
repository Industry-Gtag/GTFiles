using System;

// Token: 0x02000BE9 RID: 3049
internal struct PlayerAgeGateWarningStatus
{
	// Token: 0x04005FCA RID: 24522
	public string header;

	// Token: 0x04005FCB RID: 24523
	public string body;

	// Token: 0x04005FCC RID: 24524
	public string leftButtonText;

	// Token: 0x04005FCD RID: 24525
	public string rightButtonText;

	// Token: 0x04005FCE RID: 24526
	public WarningButtonResult leftButtonResult;

	// Token: 0x04005FCF RID: 24527
	public WarningButtonResult rightButtonResult;

	// Token: 0x04005FD0 RID: 24528
	public WarningButtonResult noWarningResult;

	// Token: 0x04005FD1 RID: 24529
	public EImageVisibility showImage;

	// Token: 0x04005FD2 RID: 24530
	public Action onLeftButtonPressedAction;

	// Token: 0x04005FD3 RID: 24531
	public Action onRightButtonPressedAction;
}
