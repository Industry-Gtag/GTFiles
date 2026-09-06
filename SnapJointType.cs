using System;

// Token: 0x020006FE RID: 1790
[Flags]
public enum SnapJointType
{
	// Token: 0x040039A7 RID: 14759
	None = 0,
	// Token: 0x040039A8 RID: 14760
	HandL = 1,
	// Token: 0x040039A9 RID: 14761
	HandR = 4,
	// Token: 0x040039AA RID: 14762
	Chest = 8,
	// Token: 0x040039AB RID: 14763
	Back = 16,
	// Token: 0x040039AC RID: 14764
	Head = 32,
	// Token: 0x040039AD RID: 14765
	Holster = 64,
	// Token: 0x040039AE RID: 14766
	ForearmL = 128,
	// Token: 0x040039AF RID: 14767
	ForearmR = 256,
	// Token: 0x040039B0 RID: 14768
	AuxHead = 512,
	// Token: 0x040039B1 RID: 14769
	AuxBody1 = 1024,
	// Token: 0x040039B2 RID: 14770
	AuxBody2 = 2048,
	// Token: 0x040039B3 RID: 14771
	AuxShoulderL = 4096,
	// Token: 0x040039B4 RID: 14772
	AuxShoulderR = 8192,
	// Token: 0x040039B5 RID: 14773
	Max = 16384
}
