using System;
using GorillaTag.Reactions;
using UnityEngine;

// Token: 0x02000E0F RID: 3599
[RequireComponent(typeof(SpawnWorldEffects))]
public class SpawnWorldEffectsTrigger : MonoBehaviour
{
	// Token: 0x06005813 RID: 22547 RVA: 0x001CAEEE File Offset: 0x001C90EE
	private void OnEnable()
	{
		if (this.swe == null)
		{
			this.swe = base.GetComponent<SpawnWorldEffects>();
		}
	}

	// Token: 0x06005814 RID: 22548 RVA: 0x001CAF0A File Offset: 0x001C910A
	private void OnTriggerEnter(Collider other)
	{
		this.spawnTime = Time.time;
		this.swe.RequestSpawn(base.transform.position);
	}

	// Token: 0x06005815 RID: 22549 RVA: 0x001CAF2D File Offset: 0x001C912D
	private void OnTriggerStay(Collider other)
	{
		if (Time.time - this.spawnTime < this.spawnCooldown)
		{
			return;
		}
		this.swe.RequestSpawn(base.transform.position);
		this.spawnTime = Time.time;
	}

	// Token: 0x04006871 RID: 26737
	private SpawnWorldEffects swe;

	// Token: 0x04006872 RID: 26738
	private float spawnTime;

	// Token: 0x04006873 RID: 26739
	[SerializeField]
	private float spawnCooldown = 1f;
}
