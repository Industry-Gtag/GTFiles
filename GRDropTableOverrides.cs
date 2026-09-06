using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000772 RID: 1906
[CreateAssetMenu(fileName = "GhostReactorDropTableOverrides", menuName = "ScriptableObjects/GhostReactorDropTableOverride")]
public class GRDropTableOverrides : ScriptableObject
{
	// Token: 0x0600305B RID: 12379 RVA: 0x001064A0 File Offset: 0x001046A0
	public GRBreakableItemSpawnConfig GetOverride(GRBreakableItemSpawnConfig table)
	{
		for (int i = 0; i < this.overrides.Count; i++)
		{
			if (this.overrides[i].table == table)
			{
				return this.overrides[i].overrideTable;
			}
		}
		return null;
	}

	// Token: 0x04003DDD RID: 15837
	public List<GRDropTableOverrides.DropTableOverride> overrides;

	// Token: 0x02000773 RID: 1907
	[Serializable]
	public class DropTableOverride
	{
		// Token: 0x04003DDE RID: 15838
		public GRBreakableItemSpawnConfig table;

		// Token: 0x04003DDF RID: 15839
		public GRBreakableItemSpawnConfig overrideTable;
	}
}
