using System;
using GorillaExtensions;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x020013EB RID: 5099
	public class CrittersActorSpawner : MonoBehaviour
	{
		// Token: 0x060080A9 RID: 32937 RVA: 0x0029D75B File Offset: 0x0029B95B
		private void Awake()
		{
			this.spawnPoint.OnSpawnChanged += this.HandleSpawnedActor;
		}

		// Token: 0x060080AA RID: 32938 RVA: 0x0029D774 File Offset: 0x0029B974
		private void OnEnable()
		{
			if (!CrittersManager.instance.actorSpawners.Contains(this))
			{
				CrittersManager.instance.actorSpawners.Add(this);
			}
		}

		// Token: 0x060080AB RID: 32939 RVA: 0x0029D79C File Offset: 0x0029B99C
		private void OnDisable()
		{
			if (CrittersManager.instance.actorSpawners.Contains(this))
			{
				CrittersManager.instance.actorSpawners.Remove(this);
			}
		}

		// Token: 0x060080AC RID: 32940 RVA: 0x0029D7C8 File Offset: 0x0029B9C8
		public void ProcessLocal()
		{
			if (!CrittersManager.instance.LocalAuthority())
			{
				return;
			}
			if (this.nextSpawnTime <= (double)Time.time)
			{
				this.nextSpawnTime = (double)(Time.time + (float)this.spawnDelay);
				if (this.currentSpawnedObject == null || !this.currentSpawnedObject.isEnabled)
				{
					this.SpawnActor();
				}
			}
			if (this.currentSpawnedObject.IsNotNull())
			{
				if (!this.currentSpawnedObject.isEnabled)
				{
					this.currentSpawnedObject = null;
					this.spawnPoint.SetSpawnedActor(null);
					return;
				}
				if (!this.insideSpawnerCheck.bounds.Contains(this.currentSpawnedObject.transform.position))
				{
					this.currentSpawnedObject.RemoveDespawnBlock();
					this.currentSpawnedObject = null;
					this.spawnPoint.SetSpawnedActor(null);
					return;
				}
				if (!this.VerifySpawnAttached())
				{
					this.currentSpawnedObject.RemoveDespawnBlock();
					this.currentSpawnedObject = null;
					this.spawnPoint.SetSpawnedActor(null);
				}
			}
		}

		// Token: 0x060080AD RID: 32941 RVA: 0x0029D8C2 File Offset: 0x0029BAC2
		public void DoReset()
		{
			this.currentSpawnedObject = null;
		}

		// Token: 0x060080AE RID: 32942 RVA: 0x0029D8CB File Offset: 0x0029BACB
		private void HandleSpawnedActor(CrittersActor spawnedActor)
		{
			this.currentSpawnedObject = spawnedActor;
		}

		// Token: 0x060080AF RID: 32943 RVA: 0x0029D8D4 File Offset: 0x0029BAD4
		private void SpawnActor()
		{
			CrittersActor crittersActor = CrittersManager.instance.SpawnActor(this.actorType, this.subActorIndex);
			this.spawnPoint.SetSpawnedActor(crittersActor);
			if (crittersActor.IsNull())
			{
				return;
			}
			if (this.attachSpawnedObjectToSpawnLocation)
			{
				crittersActor.GrabbedBy(this.spawnPoint, true, default(Quaternion), default(Vector3), false);
				return;
			}
			crittersActor.MoveActor(this.spawnPoint.transform.position, this.spawnPoint.transform.rotation, false, true, true);
			crittersActor.rb.linearVelocity = Vector3.zero;
			if (this.applyImpulseOnSpawn)
			{
				crittersActor.SetImpulse();
			}
		}

		// Token: 0x060080B0 RID: 32944 RVA: 0x0029D980 File Offset: 0x0029BB80
		private bool VerifySpawnAttached()
		{
			if (this.attachSpawnedObjectToSpawnLocation)
			{
				CrittersActor crittersActor;
				CrittersManager.instance.actorById.TryGetValue(this.currentSpawnedObject.parentActorId, out crittersActor);
				if (crittersActor.IsNull() || crittersActor != this.spawnPoint)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x040091B6 RID: 37302
		public CrittersActorSpawnerPoint spawnPoint;

		// Token: 0x040091B7 RID: 37303
		public CrittersActor currentSpawnedObject;

		// Token: 0x040091B8 RID: 37304
		public CrittersActor.CrittersActorType actorType;

		// Token: 0x040091B9 RID: 37305
		public int subActorIndex = -1;

		// Token: 0x040091BA RID: 37306
		public Collider insideSpawnerCheck;

		// Token: 0x040091BB RID: 37307
		public int spawnDelay = 5;

		// Token: 0x040091BC RID: 37308
		public bool applyImpulseOnSpawn = true;

		// Token: 0x040091BD RID: 37309
		public bool attachSpawnedObjectToSpawnLocation;

		// Token: 0x040091BE RID: 37310
		private double nextSpawnTime;
	}
}
