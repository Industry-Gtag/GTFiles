using System;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class CosmeticCritterSpawnerShadeFleeing : CosmeticCritterSpawner
{
	// Token: 0x060004DB RID: 1243 RVA: 0x0001B237 File Offset: 0x00019437
	public void SetSpawnPosition(Vector3 pos)
	{
		this.spawnPosition = pos;
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x0001B240 File Offset: 0x00019440
	public override void OnSpawn(CosmeticCritter critter)
	{
		base.OnSpawn(critter);
		(critter as CosmeticCritterShadeFleeing).SetFleePosition(this.spawnPosition, base.transform.position);
	}

	// Token: 0x04000560 RID: 1376
	private Vector3 spawnPosition;
}
