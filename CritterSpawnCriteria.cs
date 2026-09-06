using System;
using UnityEngine;

// Token: 0x02000073 RID: 115
public class CritterSpawnCriteria : ScriptableObject
{
	// Token: 0x060002D3 RID: 723 RVA: 0x00011384 File Offset: 0x0000F584
	public bool CanSpawn()
	{
		if (this.spawnTimings.Length == 0)
		{
			return true;
		}
		string currentTimeOfDay = BetterDayNightManager.instance.currentTimeOfDay;
		string[] array = this.spawnTimings;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == currentTimeOfDay)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04000344 RID: 836
	public string[] spawnTimings;
}
