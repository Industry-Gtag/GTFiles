using System;
using GorillaTag;
using GorillaTag.Reactions;
using UnityEngine;

// Token: 0x02000377 RID: 887
public class BasicFireSpawner : MonoBehaviour
{
	// Token: 0x060015C9 RID: 5577 RVA: 0x0007351D File Offset: 0x0007171D
	private void Awake()
	{
		this.scale = this.fireScaleMinMax.y;
	}

	// Token: 0x060015CA RID: 5578 RVA: 0x00073530 File Offset: 0x00071730
	public void InterpolateScale(float f)
	{
		this.scale = Mathf.Lerp(this.fireScaleMinMax.x, this.fireScaleMinMax.y, f);
	}

	// Token: 0x060015CB RID: 5579 RVA: 0x00073554 File Offset: 0x00071754
	public void Spawn()
	{
		if (this.firePool == null)
		{
			this.firePool = ObjectPools.instance.GetPoolByHash(in this.firePrefab);
		}
		FireManager.SpawnFire(this.firePool, base.transform.position, Vector3.up, this.scale);
	}

	// Token: 0x04001A96 RID: 6806
	[SerializeField]
	private HashWrapper firePrefab;

	// Token: 0x04001A97 RID: 6807
	[SerializeField]
	private Vector2 fireScaleMinMax = Vector2.one;

	// Token: 0x04001A98 RID: 6808
	private SinglePool firePool;

	// Token: 0x04001A99 RID: 6809
	private float scale;
}
