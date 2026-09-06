using System;
using UnityEngine;

// Token: 0x020007DB RID: 2011
public interface IGameProjectileLauncher
{
	// Token: 0x06003348 RID: 13128 RVA: 0x00002C2D File Offset: 0x00000E2D
	void OnProjectileInit(GRRangedEnemyProjectile projectile)
	{
	}

	// Token: 0x06003349 RID: 13129 RVA: 0x00002C2D File Offset: 0x00000E2D
	void OnProjectileHit(GRRangedEnemyProjectile projectile, Collision collision)
	{
	}
}
