using System;

// Token: 0x020000E2 RID: 226
public interface SIGadgetProjectileType
{
	// Token: 0x06000552 RID: 1362
	void LocalProjectileHit(SIPlayer player = null);

	// Token: 0x06000553 RID: 1363
	void NetworkedProjectileHit(object[] data);
}
