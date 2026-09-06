using System;
using Critters.Scripts;
using UnityEngine;

// Token: 0x02000051 RID: 81
public class CrittersActorSpawnerShim : MonoBehaviour
{
	// Token: 0x0600019F RID: 415 RVA: 0x0000A2A0 File Offset: 0x000084A0
	[ContextMenu("Copy Spawner Data To Shim")]
	private CrittersActorSpawner CopySpawnerDataInPrefab()
	{
		CrittersActorSpawner component = base.gameObject.GetComponent<CrittersActorSpawner>();
		this.spawnerPointTransform = component.spawnPoint.transform;
		this.actorType = component.actorType;
		this.subActorIndex = component.subActorIndex;
		this.insideSpawnerBounds = (BoxCollider)component.insideSpawnerCheck;
		this.spawnDelay = component.spawnDelay;
		this.applyImpulseOnSpawn = component.applyImpulseOnSpawn;
		this.attachSpawnedObjectToSpawnLocation = component.attachSpawnedObjectToSpawnLocation;
		this.colliderTrigger = base.gameObject.GetComponent<BoxCollider>();
		return component;
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x0000A32C File Offset: 0x0000852C
	[ContextMenu("Replace Spawner With Shim")]
	private void ReplaceSpawnerWithShim()
	{
		CrittersActorSpawner crittersActorSpawner = this.CopySpawnerDataInPrefab();
		if (crittersActorSpawner.spawnPoint.GetComponent<Rigidbody>() != null)
		{
			Object.DestroyImmediate(crittersActorSpawner.spawnPoint.GetComponent<Rigidbody>());
		}
		Object.DestroyImmediate(crittersActorSpawner.spawnPoint);
		Object.DestroyImmediate(crittersActorSpawner);
	}

	// Token: 0x040001B6 RID: 438
	public Transform spawnerPointTransform;

	// Token: 0x040001B7 RID: 439
	public CrittersActor.CrittersActorType actorType;

	// Token: 0x040001B8 RID: 440
	public int subActorIndex;

	// Token: 0x040001B9 RID: 441
	public BoxCollider insideSpawnerBounds;

	// Token: 0x040001BA RID: 442
	public int spawnDelay;

	// Token: 0x040001BB RID: 443
	public bool applyImpulseOnSpawn;

	// Token: 0x040001BC RID: 444
	public bool attachSpawnedObjectToSpawnLocation;

	// Token: 0x040001BD RID: 445
	public BoxCollider colliderTrigger;
}
