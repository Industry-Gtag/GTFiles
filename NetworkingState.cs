using System;

// Token: 0x02000395 RID: 917
public enum NetworkingState
{
	// Token: 0x04002077 RID: 8311
	IsOwner,
	// Token: 0x04002078 RID: 8312
	IsBlindClient,
	// Token: 0x04002079 RID: 8313
	IsClient,
	// Token: 0x0400207A RID: 8314
	ForcefullyTakingOver,
	// Token: 0x0400207B RID: 8315
	RequestingOwnership,
	// Token: 0x0400207C RID: 8316
	RequestingOwnershipWaitingForSight,
	// Token: 0x0400207D RID: 8317
	ForcefullyTakingOverWaitingForSight
}
