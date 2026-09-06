using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200070F RID: 1807
[CreateAssetMenu(fileName = "GhostReactorLevelGenConfig", menuName = "ScriptableObjects/GhostReactorLevelGenConfig")]
public class GhostReactorLevelGenConfig : ScriptableObject
{
	// Token: 0x06002D90 RID: 11664 RVA: 0x000F6508 File Offset: 0x000F4708
	private void OnValidate()
	{
		for (int i = 0; i < this.treeLevels.Count; i++)
		{
			GhostReactorLevelGeneratorV2.TreeLevelConfig treeLevelConfig = this.treeLevels[i];
			treeLevelConfig.minHubs = Mathf.Abs(treeLevelConfig.minHubs);
			treeLevelConfig.maxHubs = Mathf.Abs(treeLevelConfig.maxHubs);
			treeLevelConfig.minCaps = Mathf.Abs(treeLevelConfig.minCaps);
			treeLevelConfig.maxCaps = Mathf.Abs(treeLevelConfig.maxCaps);
			if (treeLevelConfig.minHubs > treeLevelConfig.maxHubs)
			{
				treeLevelConfig.maxHubs = treeLevelConfig.minHubs;
			}
			if (treeLevelConfig.minCaps > treeLevelConfig.maxCaps)
			{
				treeLevelConfig.maxCaps = treeLevelConfig.minCaps;
			}
			this.treeLevels[i] = treeLevelConfig;
		}
		GhostReactorLevelGeneratorV2.TreeLevelConfig treeLevelConfig2 = this.treeLevels[this.treeLevels.Count - 1];
		if (treeLevelConfig2.minHubs > 0 || treeLevelConfig2.maxHubs > 0)
		{
			Debug.LogError("Ghost Reactor Level Gen Setup Error: The last tree level can only spawn end caps around the furthest level of hubs. Otherwise it would spawn hubs without a further level to spawn end caps around them");
			treeLevelConfig2.minHubs = 0;
			treeLevelConfig2.maxHubs = 0;
			this.treeLevels[this.treeLevels.Count - 1] = treeLevelConfig2;
		}
		using (List<GREnemyCount>.Enumerator enumerator = this.minEnemyKills.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Count < 0)
				{
					Debug.LogError("Ghost Reactor Level Gen Setup Error: cannot have negative required enemy kills");
				}
			}
		}
	}

	// Token: 0x04003A3D RID: 14909
	public int shiftDuration;

	// Token: 0x04003A3E RID: 14910
	public int coresRequired;

	// Token: 0x04003A3F RID: 14911
	public int shiftBonus;

	// Token: 0x04003A40 RID: 14912
	public int sentientCoresRequired;

	// Token: 0x04003A41 RID: 14913
	public int maxPlayerDeaths = -1;

	// Token: 0x04003A42 RID: 14914
	public List<GREnemyCount> minEnemyKills = new List<GREnemyCount>();

	// Token: 0x04003A43 RID: 14915
	[ColorUsage(true, true)]
	public Color ambientLight = Color.black;

	// Token: 0x04003A44 RID: 14916
	public List<GhostReactorLevelGeneratorV2.TreeLevelConfig> treeLevels = new List<GhostReactorLevelGeneratorV2.TreeLevelConfig>();

	// Token: 0x04003A45 RID: 14917
	public List<GRBonusEntry> enemyGlobalBonuses = new List<GRBonusEntry>();

	// Token: 0x04003A46 RID: 14918
	public GRDropTableOverrides dropTableOverrides;
}
