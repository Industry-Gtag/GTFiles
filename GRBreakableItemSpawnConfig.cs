using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000766 RID: 1894
[CreateAssetMenu(fileName = "GhostReactorBreakableItemSpawnConfig", menuName = "ScriptableObjects/GhostReactorBreakableItemSpawnConfig")]
public class GRBreakableItemSpawnConfig : ScriptableObject
{
	// Token: 0x06002FFF RID: 12287 RVA: 0x00105140 File Offset: 0x00103340
	public bool TryForRandomItem(GameEntity spawnFromEntity, out GameEntity entity, int sanity = 0)
	{
		GRBreakableItemSpawnConfig @override = this.GetOverride(spawnFromEntity);
		if (sanity <= 5 && @override != null)
		{
			return @override.TryForRandomItem(spawnFromEntity, out entity, sanity + 1);
		}
		if (sanity > 5)
		{
			Debug.LogError("Circular override loop");
		}
		if (Random.Range(0f, 1f) < this.spawnAnythingProbability)
		{
			float num = Random.Range(0f, this.precomputedItemTotalWeight);
			float num2 = 0f;
			for (int i = 0; i < this.perItemProbabilities.Count; i++)
			{
				num2 += this.perItemProbabilities[i].probability;
				if (num2 > num || i == this.perItemProbabilities.Count - 1)
				{
					entity = this.perItemProbabilities[i].entity;
					return true;
				}
			}
		}
		entity = null;
		return false;
	}

	// Token: 0x06003000 RID: 12288 RVA: 0x00105204 File Offset: 0x00103404
	public bool TryForRandomItem(GhostReactor reactor, ref SRand srand, out GameEntity entity, int sanity = 0)
	{
		GRBreakableItemSpawnConfig @override = this.GetOverride(reactor);
		if (sanity <= 5 && @override != null)
		{
			return @override.TryForRandomItem(reactor, ref srand, out entity, sanity + 1);
		}
		if (sanity > 5)
		{
			Debug.LogError("Circular override loop");
		}
		if (srand.NextFloat(0f, 1f) < this.spawnAnythingProbability)
		{
			float num = srand.NextFloat(0f, this.precomputedItemTotalWeight);
			float num2 = 0f;
			for (int i = 0; i < this.perItemProbabilities.Count; i++)
			{
				num2 += this.perItemProbabilities[i].probability;
				if (num2 > num || i == this.perItemProbabilities.Count - 1)
				{
					entity = this.perItemProbabilities[i].entity;
					return true;
				}
			}
		}
		entity = null;
		return false;
	}

	// Token: 0x06003001 RID: 12289 RVA: 0x001052CC File Offset: 0x001034CC
	private GRBreakableItemSpawnConfig GetOverride(GameEntity entity)
	{
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(entity);
		if (ghostReactorManager == null)
		{
			return null;
		}
		return this.GetOverride(ghostReactorManager.reactor);
	}

	// Token: 0x06003002 RID: 12290 RVA: 0x001052F8 File Offset: 0x001034F8
	private GRBreakableItemSpawnConfig GetOverride(GhostReactor reactor)
	{
		if (reactor == null)
		{
			return null;
		}
		GhostReactorLevelGenConfig currLevelGenConfig = reactor.GetCurrLevelGenConfig();
		if (currLevelGenConfig == null || currLevelGenConfig.dropTableOverrides == null)
		{
			return null;
		}
		return currLevelGenConfig.dropTableOverrides.GetOverride(this);
	}

	// Token: 0x06003003 RID: 12291 RVA: 0x0010533C File Offset: 0x0010353C
	private void OnValidate()
	{
		this.precomputedItemTotalWeight = 0f;
		for (int i = 0; i < this.perItemProbabilities.Count; i++)
		{
			this.precomputedItemTotalWeight += this.perItemProbabilities[i].probability;
		}
	}

	// Token: 0x04003D89 RID: 15753
	[SerializeField]
	[Range(0f, 1f)]
	public float spawnAnythingProbability = 0.2f;

	// Token: 0x04003D8A RID: 15754
	public List<GRBreakableItemSpawnConfig.ItemProbability> perItemProbabilities = new List<GRBreakableItemSpawnConfig.ItemProbability>();

	// Token: 0x04003D8B RID: 15755
	[SerializeField]
	[ReadOnly]
	private float precomputedItemTotalWeight;

	// Token: 0x02000767 RID: 1895
	[Serializable]
	public struct ItemProbability
	{
		// Token: 0x04003D8C RID: 15756
		public GameEntity entity;

		// Token: 0x04003D8D RID: 15757
		public float probability;
	}
}
