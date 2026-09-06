using System;
using UnityEngine;

// Token: 0x02000685 RID: 1669
public abstract class CosmeticCritterSpawnerTimed : CosmeticCritterSpawnerIndependent
{
	// Token: 0x060029D8 RID: 10712 RVA: 0x000E1C4E File Offset: 0x000DFE4E
	protected override CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(5, this.spawnIntervalMinMax.x, 0.5f);
	}

	// Token: 0x060029D9 RID: 10713 RVA: 0x000E1C66 File Offset: 0x000DFE66
	public override bool CanSpawnLocal()
	{
		if (Time.time >= this.nextLocalSpawnTime)
		{
			this.nextLocalSpawnTime = Time.time + Random.Range(this.spawnIntervalMinMax.x, this.spawnIntervalMinMax.y);
			return base.CanSpawnLocal();
		}
		return false;
	}

	// Token: 0x060029DA RID: 10714 RVA: 0x000E1CA4 File Offset: 0x000DFEA4
	public override bool CanSpawnRemote(double serverTime)
	{
		return base.CanSpawnRemote(serverTime);
	}

	// Token: 0x060029DB RID: 10715 RVA: 0x000E1CAD File Offset: 0x000DFEAD
	protected override void OnEnable()
	{
		base.OnEnable();
		if (base.IsLocal)
		{
			this.nextLocalSpawnTime = Time.time + Random.Range(this.spawnIntervalMinMax.x, this.spawnIntervalMinMax.y);
		}
	}

	// Token: 0x060029DC RID: 10716 RVA: 0x000E1CE4 File Offset: 0x000DFEE4
	protected override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x04003656 RID: 13910
	[Tooltip("The minimum and maximum time to wait between spawn attempts.")]
	[SerializeField]
	private Vector2 spawnIntervalMinMax = new Vector2(2f, 5f);

	// Token: 0x04003657 RID: 13911
	[Tooltip("Currently does nothing.")]
	[SerializeField]
	[Range(0f, 1f)]
	private float spawnChance = 1f;
}
