using System;

// Token: 0x020008EA RID: 2282
[Flags]
public enum SystemProperties
{
	// Token: 0x04004C4E RID: 19534
	SwapInterval = 1,
	// Token: 0x04004C4F RID: 19535
	HalfRefreshRate = 2,
	// Token: 0x04004C50 RID: 19536
	GPULevel = 4,
	// Token: 0x04004C51 RID: 19537
	CPULevel = 8,
	// Token: 0x04004C52 RID: 19538
	Headlock = 16,
	// Token: 0x04004C53 RID: 19539
	HeadlockTranslationX = 32,
	// Token: 0x04004C54 RID: 19540
	HeadlockTranslationY = 64,
	// Token: 0x04004C55 RID: 19541
	HeadlockTranslationZ = 128,
	// Token: 0x04004C56 RID: 19542
	PhaseSyncAdditionalPadding = 256,
	// Token: 0x04004C57 RID: 19543
	PhaseSyncDelayOverride = 512,
	// Token: 0x04004C58 RID: 19544
	PhaseSyncPredictionTime = 1024,
	// Token: 0x04004C59 RID: 19545
	PhaseSync = 2048,
	// Token: 0x04004C5A RID: 19546
	RefreshRate = 4096,
	// Token: 0x04004C5B RID: 19547
	PredictionTime = 8192
}
