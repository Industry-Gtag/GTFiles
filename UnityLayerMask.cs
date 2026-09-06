using System;

// Token: 0x020003AE RID: 942
[Flags]
public enum UnityLayerMask
{
	// Token: 0x0400211B RID: 8475
	Everything = -1,
	// Token: 0x0400211C RID: 8476
	Nothing = 0,
	// Token: 0x0400211D RID: 8477
	Default = 1,
	// Token: 0x0400211E RID: 8478
	TransparentFX = 2,
	// Token: 0x0400211F RID: 8479
	IgnoreRaycast = 4,
	// Token: 0x04002120 RID: 8480
	Water = 16,
	// Token: 0x04002121 RID: 8481
	UI = 32,
	// Token: 0x04002122 RID: 8482
	MeshBakerAtlas = 64,
	// Token: 0x04002123 RID: 8483
	GorillaEquipment = 128,
	// Token: 0x04002124 RID: 8484
	GorillaBodyCollider = 256,
	// Token: 0x04002125 RID: 8485
	GorillaObject = 512,
	// Token: 0x04002126 RID: 8486
	GorillaHand = 1024,
	// Token: 0x04002127 RID: 8487
	GorillaTrigger = 2048,
	// Token: 0x04002128 RID: 8488
	MetaReportScreen = 4096,
	// Token: 0x04002129 RID: 8489
	GorillaHead = 8192,
	// Token: 0x0400212A RID: 8490
	GorillaTagCollider = 16384,
	// Token: 0x0400212B RID: 8491
	GorillaBoundary = 32768,
	// Token: 0x0400212C RID: 8492
	GorillaEquipmentContainer = 65536,
	// Token: 0x0400212D RID: 8493
	LCKHide = 131072,
	// Token: 0x0400212E RID: 8494
	GorillaInteractable = 262144,
	// Token: 0x0400212F RID: 8495
	FirstPersonOnly = 524288,
	// Token: 0x04002130 RID: 8496
	GorillaParticle = 1048576,
	// Token: 0x04002131 RID: 8497
	GorillaCosmetics = 2097152,
	// Token: 0x04002132 RID: 8498
	MirrorOnly = 4194304,
	// Token: 0x04002133 RID: 8499
	GorillaThrowable = 8388608,
	// Token: 0x04002134 RID: 8500
	GorillaHandSocket = 16777216,
	// Token: 0x04002135 RID: 8501
	GorillaCosmeticParticle = 33554432,
	// Token: 0x04002136 RID: 8502
	BuilderProp = 67108864,
	// Token: 0x04002137 RID: 8503
	NoMirror = 134217728,
	// Token: 0x04002138 RID: 8504
	GorillaSlingshotCollider = 268435456,
	// Token: 0x04002139 RID: 8505
	RopeSwing = 536870912,
	// Token: 0x0400213A RID: 8506
	Prop = 1073741824,
	// Token: 0x0400213B RID: 8507
	Bake = -2147483648
}
