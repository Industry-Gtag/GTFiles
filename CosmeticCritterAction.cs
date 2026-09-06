using System;

// Token: 0x02000681 RID: 1665
[Flags]
public enum CosmeticCritterAction
{
	// Token: 0x0400363E RID: 13886
	None = 0,
	// Token: 0x0400363F RID: 13887
	RPC = 1,
	// Token: 0x04003640 RID: 13888
	Spawn = 2,
	// Token: 0x04003641 RID: 13889
	Despawn = 4,
	// Token: 0x04003642 RID: 13890
	SpawnLinked = 8,
	// Token: 0x04003643 RID: 13891
	ShadeHeartbeat = 16
}
