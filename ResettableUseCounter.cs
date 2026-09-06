using System;

// Token: 0x02000138 RID: 312
public struct ResettableUseCounter
{
	// Token: 0x060007CA RID: 1994 RVA: 0x0002AB48 File Offset: 0x00028D48
	public ResettableUseCounter(int maxRegularUses, int maxSuperchargeUses, Action<bool> onReadyChanged = null)
	{
		this.maxRegularUses = maxRegularUses;
		this.maxSuperchargeUses = maxSuperchargeUses;
		this.usesRemaining = maxRegularUses;
		this.onReadyChanged = onReadyChanged;
	}

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x060007CB RID: 1995 RVA: 0x0002AB66 File Offset: 0x00028D66
	public bool IsReady
	{
		get
		{
			return this.usesRemaining > 0;
		}
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x0002AB74 File Offset: 0x00028D74
	public bool TryUse()
	{
		if (!this.IsReady)
		{
			return false;
		}
		SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		bool flag = activeSuperInfectionManager != null && activeSuperInfectionManager.IsSupercharged;
		if (this.usesRemaining > this.maxRegularUses && !flag)
		{
			this.usesRemaining = this.maxRegularUses;
		}
		this.usesRemaining--;
		if (!this.IsReady)
		{
			Action<bool> action = this.onReadyChanged;
			if (action != null)
			{
				action(false);
			}
		}
		return true;
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x0002ABE3 File Offset: 0x00028DE3
	public void Reset()
	{
		bool isReady = this.IsReady;
		this.usesRemaining = this.maxSuperchargeUses;
		if (!isReady)
		{
			Action<bool> action = this.onReadyChanged;
			if (action == null)
			{
				return;
			}
			action(true);
		}
	}

	// Token: 0x040009D9 RID: 2521
	private int usesRemaining;

	// Token: 0x040009DA RID: 2522
	private int maxRegularUses;

	// Token: 0x040009DB RID: 2523
	private int maxSuperchargeUses;

	// Token: 0x040009DC RID: 2524
	private Action<bool> onReadyChanged;
}
