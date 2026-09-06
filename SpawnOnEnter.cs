using System;
using UnityEngine;

// Token: 0x02000200 RID: 512
public class SpawnOnEnter : MonoBehaviour
{
	// Token: 0x06000D72 RID: 3442 RVA: 0x00049B30 File Offset: 0x00047D30
	public void OnTriggerEnter(Collider other)
	{
		if (Time.time > this.lastSpawnTime + this.cooldown)
		{
			this.lastSpawnTime = Time.time;
			ObjectPools.instance.Instantiate(this.prefab, other.transform.position, true);
		}
	}

	// Token: 0x04001021 RID: 4129
	public GameObject prefab;

	// Token: 0x04001022 RID: 4130
	public float cooldown = 0.1f;

	// Token: 0x04001023 RID: 4131
	private float lastSpawnTime;
}
