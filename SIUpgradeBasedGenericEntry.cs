using System;
using UnityEngine;

// Token: 0x0200011F RID: 287
[Serializable]
internal struct SIUpgradeBasedGenericEntry<T>
{
	// Token: 0x0600072A RID: 1834 RVA: 0x00028FDC File Offset: 0x000271DC
	public bool IsActive(SIUpgradeSet withUpgrades)
	{
		bool flag = true;
		if (this.activeRequirements.Length != 0)
		{
			flag = false;
			foreach (SIUpgradeType siupgradeType in this.activeRequirements)
			{
				if (withUpgrades.Contains(siupgradeType))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			foreach (SIUpgradeType siupgradeType2 in this.inactiveRequirements)
			{
				if (withUpgrades.Contains(siupgradeType2))
				{
					flag = false;
					break;
				}
			}
		}
		return flag;
	}

	// Token: 0x04000913 RID: 2323
	public T value;

	// Token: 0x04000914 RID: 2324
	[Tooltip("For the objects to become activated, you must match AT LEAST ONE appearRequirement (if there are any), and not match any disappearRequirements.")]
	public SIUpgradeType[] activeRequirements;

	// Token: 0x04000915 RID: 2325
	[Tooltip("For the objects to become deactivated, you must match AT LEAST ONE disappearRequirement (if there are any).")]
	public SIUpgradeType[] inactiveRequirements;
}
