using System;

// Token: 0x020002CE RID: 718
[Flags]
public enum GestureNodeFlags : uint
{
	// Token: 0x0400165E RID: 5726
	None = 0U,
	// Token: 0x0400165F RID: 5727
	HandLeft = 1U,
	// Token: 0x04001660 RID: 5728
	HandRight = 2U,
	// Token: 0x04001661 RID: 5729
	HandOpen = 4U,
	// Token: 0x04001662 RID: 5730
	HandClosed = 8U,
	// Token: 0x04001663 RID: 5731
	DigitOpen = 16U,
	// Token: 0x04001664 RID: 5732
	DigitClosed = 32U,
	// Token: 0x04001665 RID: 5733
	DigitBent = 64U,
	// Token: 0x04001666 RID: 5734
	TowardFace = 128U,
	// Token: 0x04001667 RID: 5735
	AwayFromFace = 256U,
	// Token: 0x04001668 RID: 5736
	AxisWorldUp = 512U,
	// Token: 0x04001669 RID: 5737
	AxisWorldDown = 1024U
}
