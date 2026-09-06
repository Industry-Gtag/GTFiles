using System;
using UnityEngine;

// Token: 0x02000824 RID: 2084
public class GRToolUpgrade : ScriptableObject
{
	// Token: 0x040045CA RID: 17866
	public string upgradeName;

	// Token: 0x040045CB RID: 17867
	public string description;

	// Token: 0x040045CC RID: 17868
	public string upgradeId;

	// Token: 0x040045CD RID: 17869
	[SerializeField]
	public GRToolUpgrade.ToolUpgradeLevel[] upgradeLevels;

	// Token: 0x02000825 RID: 2085
	[Serializable]
	public struct ToolUpgradeLevel
	{
		// Token: 0x040045CE RID: 17870
		[SerializeField]
		public int Cost;

		// Token: 0x040045CF RID: 17871
		[SerializeField]
		public float upgradeAmount;
	}
}
