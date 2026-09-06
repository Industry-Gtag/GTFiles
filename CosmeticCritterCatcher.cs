using System;
using UnityEngine;

// Token: 0x0200067F RID: 1663
public abstract class CosmeticCritterCatcher : CosmeticCritterHoldable
{
	// Token: 0x060029A1 RID: 10657 RVA: 0x000E116F File Offset: 0x000DF36F
	public CosmeticCritterSpawner GetLinkedSpawner()
	{
		return this.optionalLinkedSpawner;
	}

	// Token: 0x060029A2 RID: 10658
	public abstract CosmeticCritterAction GetLocalCatchAction(CosmeticCritter critter);

	// Token: 0x060029A3 RID: 10659 RVA: 0x000E1177 File Offset: 0x000DF377
	public virtual bool ValidateRemoteCatchAction(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		return this.callLimiter.CheckCallServerTime(serverTime);
	}

	// Token: 0x060029A4 RID: 10660
	public abstract void OnCatch(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime);

	// Token: 0x060029A5 RID: 10661 RVA: 0x000E1185 File Offset: 0x000DF385
	protected override void OnEnable()
	{
		base.OnEnable();
		CosmeticCritterManager.Instance.RegisterCatcher(this);
	}

	// Token: 0x060029A6 RID: 10662 RVA: 0x000E1198 File Offset: 0x000DF398
	protected override void OnDisable()
	{
		base.OnDisable();
		CosmeticCritterManager.Instance.UnregisterCatcher(this);
	}

	// Token: 0x04003639 RID: 13881
	[SerializeField]
	[Tooltip("If this catcher is capable of spawning immediately after catching, the linked spawner must be assigned here.")]
	protected CosmeticCritterSpawner optionalLinkedSpawner;
}
