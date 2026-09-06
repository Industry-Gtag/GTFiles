using System;
using GorillaTagScripts.GhostReactor;

// Token: 0x0200079D RID: 1949
[Serializable]
public struct GREnemyCount
{
	// Token: 0x0600319F RID: 12703 RVA: 0x0010DF34 File Offset: 0x0010C134
	public GREnemyType GetEnemyType()
	{
		if (this.EnemyType == GREnemyType.MoonBoss_Phase1 || this.EnemyType == GREnemyType.MoonBoss_Phase2)
		{
			return GREnemyType.MoonBoss;
		}
		return this.EnemyType;
	}

	// Token: 0x060031A0 RID: 12704 RVA: 0x0010DF54 File Offset: 0x0010C154
	public string GetEnemyName()
	{
		if (this.GetEnemyType() == GREnemyType.MoonBoss)
		{
			return "Meteor Monster";
		}
		return this.GetEnemyType().ToString();
	}

	// Token: 0x04003FCE RID: 16334
	public GREnemyType EnemyType;

	// Token: 0x04003FCF RID: 16335
	public int Count;
}
