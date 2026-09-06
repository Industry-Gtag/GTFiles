using System;
using UnityEngine;

// Token: 0x020007FF RID: 2047
public class GRSummonerEgg : MonoBehaviour
{
	// Token: 0x06003457 RID: 13399 RVA: 0x0011F866 File Offset: 0x0011DA66
	private void Awake()
	{
		this.summonedEntity = base.GetComponent<GRSummonedEntity>();
	}

	// Token: 0x06003458 RID: 13400 RVA: 0x0011F874 File Offset: 0x0011DA74
	private void Start()
	{
		this.hatchTime = Random.Range(this.minHatchTime, this.maxHatchTime);
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = base.transform.position;
			component.rotation = base.transform.rotation;
			component.linearVelocity = Vector3.up * 2f;
			component.angularVelocity = Vector3.zero;
			component.constraints |= (RigidbodyConstraints)10;
		}
		base.Invoke("HatchEgg", this.hatchTime);
	}

	// Token: 0x06003459 RID: 13401 RVA: 0x0011F914 File Offset: 0x0011DB14
	public void HatchEgg()
	{
		GRBreakable component = base.GetComponent<GRBreakable>();
		if (component)
		{
			component.BreakLocal();
		}
		if (this.entity.IsAuthority())
		{
			Vector3 vector = this.entity.transform.position + this.spawnOffset;
			Quaternion identity = Quaternion.identity;
			GameEntityManager gameEntityManager = GhostReactorManager.Get(this.entity).gameEntityManager;
			GameEntity gameEntity = this.entityPrefabToSpawn;
			if (this.lootTableToSpawn != null)
			{
				this.lootTableToSpawn.TryForRandomItem(this.entity, out gameEntity, 0);
			}
			gameEntityManager.RequestCreateItem(gameEntity.name.GetStaticHash(), vector, identity, 0L, (this.summonedEntity != null) ? this.summonedEntity.GetSummonerID() : GameEntityId.Invalid);
		}
		base.Invoke("DestroySelf", 2f);
		this.hatchSound.Play(this.hatchAudio);
	}

	// Token: 0x0600345A RID: 13402 RVA: 0x0011F9F8 File Offset: 0x0011DBF8
	public void DestroySelf()
	{
		if (this.entity.IsAuthority())
		{
			this.entity.manager.RequestDestroyItem(this.entity.id);
		}
	}

	// Token: 0x0400441C RID: 17436
	public GameEntity entity;

	// Token: 0x0400441D RID: 17437
	public AudioSource hatchAudio;

	// Token: 0x0400441E RID: 17438
	public AbilitySound hatchSound;

	// Token: 0x0400441F RID: 17439
	public GameEntity entityPrefabToSpawn;

	// Token: 0x04004420 RID: 17440
	public GRBreakableItemSpawnConfig lootTableToSpawn;

	// Token: 0x04004421 RID: 17441
	public Vector3 spawnOffset = new Vector3(0f, 0f, 0.3f);

	// Token: 0x04004422 RID: 17442
	public float minHatchTime = 3f;

	// Token: 0x04004423 RID: 17443
	public float maxHatchTime = 6f;

	// Token: 0x04004424 RID: 17444
	private float hatchTime = 2f;

	// Token: 0x04004425 RID: 17445
	private GRSummonedEntity summonedEntity;
}
