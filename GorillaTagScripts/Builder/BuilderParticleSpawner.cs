using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001030 RID: 4144
	public class BuilderParticleSpawner : MonoBehaviour
	{
		// Token: 0x06006726 RID: 26406 RVA: 0x002123D0 File Offset: 0x002105D0
		private void Start()
		{
			this.spawnTrigger.onTriggerFirstEntered += this.OnEnter;
			this.spawnTrigger.onTriggerLastExited += this.OnExit;
		}

		// Token: 0x06006727 RID: 26407 RVA: 0x00212400 File Offset: 0x00210600
		private void OnDestroy()
		{
			if (this.spawnTrigger != null)
			{
				this.spawnTrigger.onTriggerFirstEntered -= this.OnEnter;
				this.spawnTrigger.onTriggerLastExited -= this.OnExit;
			}
		}

		// Token: 0x06006728 RID: 26408 RVA: 0x00212440 File Offset: 0x00210640
		public void TrySpawning()
		{
			if (Time.time > this.lastSpawnTime + this.cooldown)
			{
				this.lastSpawnTime = Time.time;
				ObjectPools.instance.Instantiate(this.prefab, this.spawnLocation.position, this.spawnLocation.rotation, this.myPiece.GetScale(), true);
			}
		}

		// Token: 0x06006729 RID: 26409 RVA: 0x0021249F File Offset: 0x0021069F
		private void OnEnter()
		{
			if (this.spawnOnEnter)
			{
				this.TrySpawning();
			}
		}

		// Token: 0x0600672A RID: 26410 RVA: 0x002124AF File Offset: 0x002106AF
		private void OnExit()
		{
			if (this.spawnOnExit)
			{
				this.TrySpawning();
			}
		}

		// Token: 0x04007608 RID: 30216
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04007609 RID: 30217
		public GameObject prefab;

		// Token: 0x0400760A RID: 30218
		public float cooldown = 0.1f;

		// Token: 0x0400760B RID: 30219
		private float lastSpawnTime;

		// Token: 0x0400760C RID: 30220
		[SerializeField]
		private BuilderSmallMonkeTrigger spawnTrigger;

		// Token: 0x0400760D RID: 30221
		[SerializeField]
		private bool spawnOnEnter = true;

		// Token: 0x0400760E RID: 30222
		[SerializeField]
		private bool spawnOnExit;

		// Token: 0x0400760F RID: 30223
		[SerializeField]
		private Transform spawnLocation;
	}
}
