using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class TestSpawnGadget : MonoBehaviour
{
	// Token: 0x06000A98 RID: 2712 RVA: 0x00039050 File Offset: 0x00037250
	public void Spawn(GameEntityManager gameEntityManager)
	{
		SIUpgradeSet siupgradeSet = default(SIUpgradeSet);
		foreach (TestSpawnGadget.SpawnTypeWithUpgrades spawnTypeWithUpgrades in this.testSpawnList)
		{
			if (!(spawnTypeWithUpgrades.prefab == null))
			{
				siupgradeSet.Clear();
				foreach (SIUpgradeType siupgradeType in spawnTypeWithUpgrades.upgrades)
				{
					siupgradeSet.Add(siupgradeType);
				}
				this.SpawnGadgetBatch(gameEntityManager, spawnTypeWithUpgrades.prefab, siupgradeSet);
			}
		}
		if (!this.spawnAllGadgets)
		{
			return;
		}
		siupgradeSet.Clear();
		foreach (GameEntity gameEntity in gameEntityManager.tempFactoryItems)
		{
			if (!this.skipEntityList.Contains(gameEntity))
			{
				this.SpawnGadgetBatch(gameEntityManager, gameEntity, siupgradeSet);
			}
		}
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x00039158 File Offset: 0x00037358
	private void SpawnGadgetBatch(GameEntityManager gameEntityManager, GameEntity entityToSpawn, SIUpgradeSet upgrades)
	{
		for (int i = 0; i < this.spawnBatchSize; i++)
		{
			gameEntityManager.RequestCreateItem(entityToSpawn.gameObject.name.GetStaticHash(), base.transform.position + Random.insideUnitSphere, base.transform.rotation, (long)upgrades.GetBits() << 32);
		}
	}

	// Token: 0x04000CDA RID: 3290
	public int spawnBatchSize = 4;

	// Token: 0x04000CDB RID: 3291
	public List<TestSpawnGadget.SpawnTypeWithUpgrades> testSpawnList = new List<TestSpawnGadget.SpawnTypeWithUpgrades>();

	// Token: 0x04000CDC RID: 3292
	public bool spawnAllGadgets;

	// Token: 0x04000CDD RID: 3293
	public List<GameEntity> skipEntityList = new List<GameEntity>();

	// Token: 0x0200018C RID: 396
	[Serializable]
	public struct SpawnTypeWithUpgrades
	{
		// Token: 0x04000CDE RID: 3294
		public GameEntity prefab;

		// Token: 0x04000CDF RID: 3295
		public SIUpgradeType[] upgrades;
	}
}
