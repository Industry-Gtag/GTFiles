using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200070D RID: 1805
[CreateAssetMenu(fileName = "GhostReactorLevelDepthConfig", menuName = "ScriptableObjects/GhostReactorLevelDepthConfig")]
public class GhostReactorLevelDepthConfig : ScriptableObject
{
	// Token: 0x04003A38 RID: 14904
	public string displayName;

	// Token: 0x04003A39 RID: 14905
	public List<GhostReactorLevelGenConfig> configGenOptions = new List<GhostReactorLevelGenConfig>();

	// Token: 0x04003A3A RID: 14906
	public List<GhostReactorLevelDepthConfig.LevelOption> options = new List<GhostReactorLevelDepthConfig.LevelOption>();

	// Token: 0x0200070E RID: 1806
	[Serializable]
	public class LevelOption
	{
		// Token: 0x04003A3B RID: 14907
		public int weight = 100;

		// Token: 0x04003A3C RID: 14908
		public GhostReactorLevelGenConfig levelConfig;
	}
}
