using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000769 RID: 1897
public class GRCollectibleDispenser : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x17000484 RID: 1156
	// (get) Token: 0x0600300B RID: 12299 RVA: 0x0010541A File Offset: 0x0010361A
	public bool CollectibleAlreadySpawned
	{
		get
		{
			return this.currentCollectible != null;
		}
	}

	// Token: 0x17000485 RID: 1157
	// (get) Token: 0x0600300C RID: 12300 RVA: 0x00105428 File Offset: 0x00103628
	public bool ReadyToDispenseNewCollectible
	{
		get
		{
			double num = (double)this.collectibleRespawnTimeMinutes * 60.0;
			bool flag = (ulong)this.collectiblesDispensed < (ulong)((long)this.maxDispenseCount);
			return !this.CollectibleAlreadySpawned && flag && Time.timeAsDouble - this.collectibleDispenseRequestTime > num && Time.timeAsDouble - this.collectibleDispenseTime > num && Time.timeAsDouble - this.collectibleCollectedTime > num;
		}
	}

	// Token: 0x0600300D RID: 12301 RVA: 0x00105494 File Offset: 0x00103694
	public void OnEntityInit()
	{
		GhostReactor reactor = GhostReactorManager.Get(this.gameEntity).reactor;
		if (reactor != null)
		{
			reactor.collectibleDispensers.Add(this);
		}
	}

	// Token: 0x0600300E RID: 12302 RVA: 0x001054C8 File Offset: 0x001036C8
	public void OnEntityDestroy()
	{
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(this.gameEntity);
		if (ghostReactorManager != null && ghostReactorManager.reactor != null)
		{
			ghostReactorManager.reactor.collectibleDispensers.Remove(this);
		}
	}

	// Token: 0x0600300F RID: 12303 RVA: 0x0010550C File Offset: 0x0010370C
	public void OnEntityStateChange(long prevState, long nextState)
	{
		uint num = this.collectiblesDispensed;
		uint num2 = this.collectiblesCollected;
		this.collectiblesDispensed = (uint)(nextState >> 32);
		this.collectiblesCollected = (uint)(nextState & (long)((ulong)(-1)));
		if (num != this.collectiblesDispensed)
		{
			this.collectibleDispenseTime = Time.timeAsDouble;
		}
		if (num2 != this.collectiblesCollected)
		{
			this.collectibleCollectedTime = Time.timeAsDouble;
		}
		if ((ulong)this.collectiblesCollected >= (ulong)((long)this.maxDispenseCount))
		{
			this.stillDispensingModel.gameObject.SetActive(false);
			this.fullyConsumedModel.gameObject.SetActive(true);
		}
	}

	// Token: 0x06003010 RID: 12304 RVA: 0x00105598 File Offset: 0x00103798
	public void RequestDispenseCollectible()
	{
		if (this.ReadyToDispenseNewCollectible && this.gameEntity.IsAuthority())
		{
			this.gameEntity.manager.RequestCreateItem(this.collectiblePrefab.name.GetStaticHash(), this.spawnLocation.position, this.spawnLocation.rotation, (long)this.gameEntity.manager.GetNetIdFromEntityId(this.gameEntity.id));
			this.collectiblesDispensed += 1U;
			this.collectibleDispenseTime = Time.timeAsDouble;
			long num = (long)((ulong)this.collectiblesDispensed);
			long num2 = (long)((ulong)this.collectiblesCollected);
			long num3 = (num << 32) | num2;
			this.gameEntity.RequestState(this.gameEntity.id, num3);
		}
	}

	// Token: 0x06003011 RID: 12305 RVA: 0x00105658 File Offset: 0x00103858
	public void OnCollectibleConsumed()
	{
		if (this.currentCollectible != null && this.currentCollectible.IsNotNull())
		{
			GRCollectible grcollectible = this.currentCollectible;
			grcollectible.OnCollected = (Action)Delegate.Remove(grcollectible.OnCollected, new Action(this.OnCollectibleConsumed));
			GameEntity entity = this.currentCollectible.entity;
			entity.OnGrabbed = (Action)Delegate.Remove(entity.OnGrabbed, new Action(this.OnCollectibleConsumed));
			this.currentCollectible = null;
		}
		this.collectiblesCollected += 1U;
		this.collectibleCollectedTime = Time.timeAsDouble;
		if (this.gameEntity.IsAuthority())
		{
			long num = (long)((ulong)this.collectiblesDispensed);
			long num2 = (long)((ulong)this.collectiblesCollected);
			long num3 = (num << 32) | num2;
			this.gameEntity.RequestState(this.gameEntity.id, num3);
		}
		if ((ulong)this.collectiblesCollected >= (ulong)((long)this.maxDispenseCount))
		{
			this.dispenserExhaustedEffect.Play();
			this.audioSource.PlayOneShot(this.dispenserExhaustedClip, this.dispenserExhaustedVolume);
			this.stillDispensingModel.gameObject.SetActive(false);
			this.fullyConsumedModel.gameObject.SetActive(true);
			return;
		}
		this.collectibleTakenEffect.Play();
		this.audioSource.PlayOneShot(this.collectibleTakenClip, this.collectibleTakenVolume);
	}

	// Token: 0x06003012 RID: 12306 RVA: 0x001057A4 File Offset: 0x001039A4
	public void GetSpawnedCollectible(GRCollectible collectible)
	{
		this.currentCollectible = collectible;
		collectible.OnCollected = (Action)Delegate.Combine(collectible.OnCollected, new Action(this.OnCollectibleConsumed));
		GameEntity entity = collectible.entity;
		entity.OnGrabbed = (Action)Delegate.Combine(entity.OnGrabbed, new Action(this.OnCollectibleConsumed));
	}

	// Token: 0x04003D92 RID: 15762
	public GameEntity gameEntity;

	// Token: 0x04003D93 RID: 15763
	public GameEntity collectiblePrefab;

	// Token: 0x04003D94 RID: 15764
	public Transform spawnLocation;

	// Token: 0x04003D95 RID: 15765
	public LayerMask collectibleLayerMask;

	// Token: 0x04003D96 RID: 15766
	public float collectibleRespawnTimeMinutes = 1.5f;

	// Token: 0x04003D97 RID: 15767
	public int maxDispenseCount = 3;

	// Token: 0x04003D98 RID: 15768
	public AudioSource audioSource;

	// Token: 0x04003D99 RID: 15769
	public Transform stillDispensingModel;

	// Token: 0x04003D9A RID: 15770
	public Transform fullyConsumedModel;

	// Token: 0x04003D9B RID: 15771
	public ParticleSystem collectibleTakenEffect;

	// Token: 0x04003D9C RID: 15772
	public AudioClip collectibleTakenClip;

	// Token: 0x04003D9D RID: 15773
	public float collectibleTakenVolume;

	// Token: 0x04003D9E RID: 15774
	public ParticleSystem dispenserExhaustedEffect;

	// Token: 0x04003D9F RID: 15775
	public AudioClip dispenserExhaustedClip;

	// Token: 0x04003DA0 RID: 15776
	public float dispenserExhaustedVolume;

	// Token: 0x04003DA1 RID: 15777
	private GRCollectible currentCollectible;

	// Token: 0x04003DA2 RID: 15778
	private Coroutine getSpawnedCollectibleCoroutine;

	// Token: 0x04003DA3 RID: 15779
	private static Collider[] overlapColliders = new Collider[10];

	// Token: 0x04003DA4 RID: 15780
	private uint collectiblesDispensed;

	// Token: 0x04003DA5 RID: 15781
	private uint collectiblesCollected;

	// Token: 0x04003DA6 RID: 15782
	private double collectibleDispenseRequestTime = -10000.0;

	// Token: 0x04003DA7 RID: 15783
	private double collectibleDispenseTime = -10000.0;

	// Token: 0x04003DA8 RID: 15784
	private double collectibleCollectedTime = -10000.0;
}
