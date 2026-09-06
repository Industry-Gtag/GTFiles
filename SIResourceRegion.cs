using System;

// Token: 0x02000162 RID: 354
public class SIResourceRegion : SpawnRegion<GameEntity, SIResourceRegion>
{
	// Token: 0x170000BC RID: 188
	// (get) Token: 0x06000950 RID: 2384 RVA: 0x0003254C File Offset: 0x0003074C
	// (set) Token: 0x06000951 RID: 2385 RVA: 0x00032554 File Offset: 0x00030754
	public float LastSpawnTime { get; set; }

	// Token: 0x04000B68 RID: 2920
	public SIResource resourcePrefab;
}
