using System;
using UnityEngine;

// Token: 0x02000683 RID: 1667
public abstract class CosmeticCritterSpawner : CosmeticCritterHoldable
{
	// Token: 0x060029CA RID: 10698 RVA: 0x000E1B7C File Offset: 0x000DFD7C
	public GameObject GetCritterPrefab()
	{
		return this.critterPrefab;
	}

	// Token: 0x060029CB RID: 10699 RVA: 0x000E1B84 File Offset: 0x000DFD84
	public CosmeticCritter GetCritter()
	{
		return this.cachedCritter;
	}

	// Token: 0x060029CC RID: 10700 RVA: 0x000E1B8C File Offset: 0x000DFD8C
	public Type GetCritterType()
	{
		return this.cachedType;
	}

	// Token: 0x060029CD RID: 10701 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void SetRandomVariables(CosmeticCritter critter)
	{
	}

	// Token: 0x060029CE RID: 10702 RVA: 0x000E1B94 File Offset: 0x000DFD94
	public virtual void OnSpawn(CosmeticCritter critter)
	{
		this.numCritters++;
	}

	// Token: 0x060029CF RID: 10703 RVA: 0x000E1BA4 File Offset: 0x000DFDA4
	public virtual void OnDespawn(CosmeticCritter critter)
	{
		this.numCritters = Math.Max(this.numCritters - 1, 0);
	}

	// Token: 0x060029D0 RID: 10704 RVA: 0x000E1BBA File Offset: 0x000DFDBA
	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.cachedCritter == null)
		{
			this.cachedCritter = this.critterPrefab.GetComponent<CosmeticCritter>();
			this.cachedType = this.cachedCritter.GetType();
		}
	}

	// Token: 0x060029D1 RID: 10705 RVA: 0x000E1BF2 File Offset: 0x000DFDF2
	protected override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x04003650 RID: 13904
	[Tooltip("The critter prefab to spawn.")]
	[SerializeField]
	protected GameObject critterPrefab;

	// Token: 0x04003651 RID: 13905
	[Tooltip("The maximum number of critters that this spawner can have active at once.")]
	[SerializeField]
	protected int maxCritters;

	// Token: 0x04003652 RID: 13906
	protected CosmeticCritter cachedCritter;

	// Token: 0x04003653 RID: 13907
	protected Type cachedType;

	// Token: 0x04003654 RID: 13908
	protected int numCritters;

	// Token: 0x04003655 RID: 13909
	protected float nextLocalSpawnTime;
}
