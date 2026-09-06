using System;
using UnityEngine;

// Token: 0x02000823 RID: 2083
public class GRToolUnlock : ScriptableObject
{
	// Token: 0x040045C5 RID: 17861
	public string toolName;

	// Token: 0x040045C6 RID: 17862
	public string toolId;

	// Token: 0x040045C7 RID: 17863
	public int unlockLevel;

	// Token: 0x040045C8 RID: 17864
	public int unlockCost;

	// Token: 0x040045C9 RID: 17865
	public GRToolUpgrade[] toolUpgrades;
}
