using System;

// Token: 0x0200011E RID: 286
[Serializable]
internal struct SIUpgradeBasedGeneric<T>
{
	// Token: 0x06000729 RID: 1833 RVA: 0x00028F84 File Offset: 0x00027184
	public bool TryGetActiveValue(SIUpgradeSet withUpgrades, out T out_value)
	{
		out_value = default(T);
		bool flag = false;
		for (int i = 0; i < this.entries.Length; i++)
		{
			if (this.entries[i].IsActive(withUpgrades))
			{
				flag = true;
				out_value = this.entries[i].value;
			}
		}
		return flag;
	}

	// Token: 0x04000912 RID: 2322
	public SIUpgradeBasedGenericEntry<T>[] entries;
}
