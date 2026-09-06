using System;

// Token: 0x02000684 RID: 1668
public class CosmeticCritterSpawnerIndependent : CosmeticCritterSpawner
{
	// Token: 0x060029D3 RID: 10707 RVA: 0x000E1BFA File Offset: 0x000DFDFA
	public virtual bool CanSpawnLocal()
	{
		return this.numCritters < this.maxCritters;
	}

	// Token: 0x060029D4 RID: 10708 RVA: 0x000E1C0A File Offset: 0x000DFE0A
	public virtual bool CanSpawnRemote(double serverTime)
	{
		return this.numCritters < this.maxCritters && this.callLimiter.CheckCallServerTime(serverTime);
	}

	// Token: 0x060029D5 RID: 10709 RVA: 0x000E1C28 File Offset: 0x000DFE28
	protected override void OnEnable()
	{
		base.OnEnable();
		CosmeticCritterManager.Instance.RegisterIndependentSpawner(this);
	}

	// Token: 0x060029D6 RID: 10710 RVA: 0x000E1C3B File Offset: 0x000DFE3B
	protected override void OnDisable()
	{
		base.OnDisable();
		CosmeticCritterManager.Instance.UnregisterIndependentSpawner(this);
	}
}
