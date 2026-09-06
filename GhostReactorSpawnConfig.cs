using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000728 RID: 1832
[CreateAssetMenu(fileName = "GhostReactorSpawnConfig", menuName = "ScriptableObjects/GhostReactorSpawnConfig")]
public class GhostReactorSpawnConfig : ScriptableObject
{
	// Token: 0x04003B77 RID: 15223
	public List<GhostReactorSpawnConfig.EntitySpawnGroup> entitySpawnGroups;

	// Token: 0x02000729 RID: 1833
	public enum SpawnPointType
	{
		// Token: 0x04003B79 RID: 15225
		Enemy,
		// Token: 0x04003B7A RID: 15226
		Collectible,
		// Token: 0x04003B7B RID: 15227
		Barrier,
		// Token: 0x04003B7C RID: 15228
		HazardLiquid,
		// Token: 0x04003B7D RID: 15229
		Phantom,
		// Token: 0x04003B7E RID: 15230
		Pest,
		// Token: 0x04003B7F RID: 15231
		Crate,
		// Token: 0x04003B80 RID: 15232
		Tool,
		// Token: 0x04003B81 RID: 15233
		ChaosSeed,
		// Token: 0x04003B82 RID: 15234
		HazardTower,
		// Token: 0x04003B83 RID: 15235
		MiniBoss,
		// Token: 0x04003B84 RID: 15236
		SpawnPointTypeCount
	}

	// Token: 0x0200072A RID: 1834
	[Serializable]
	public struct EntitySpawnGroup
	{
		// Token: 0x04003B85 RID: 15237
		public GhostReactorSpawnConfig.SpawnPointType spawnPointType;

		// Token: 0x04003B86 RID: 15238
		public GameEntity entity;

		// Token: 0x04003B87 RID: 15239
		public GRBreakableItemSpawnConfig randomEntity;

		// Token: 0x04003B88 RID: 15240
		public int spawnCount;
	}
}
