using System;
using UnityEngine;

// Token: 0x020007DC RID: 2012
public class GRRangedEnemyProjectile : MonoBehaviour, IGameEntityComponent, IGameHittable, IGameHitter
{
	// Token: 0x0600334A RID: 13130 RVA: 0x00118A80 File Offset: 0x00116C80
	private void Awake()
	{
		this.particleSystem = base.GetComponentInChildren<ParticleSystem>();
		this.audioSource = base.GetComponentInChildren<AudioSource>();
		this.meshRenderer = base.GetComponentInChildren<MeshRenderer>();
		this.hittable = base.GetComponentInChildren<GameHittable>();
		this.projectileRigidbody = base.GetComponent<Rigidbody>();
		this.entity = base.GetComponent<GameEntity>();
	}

	// Token: 0x0600334B RID: 13131 RVA: 0x00118AD8 File Offset: 0x00116CD8
	private void Start()
	{
		if (this.projectileRigidbody != null)
		{
			this.projectileRigidbody.linearVelocity = base.transform.forward * this.projectileSpeed;
		}
		this.projectileHasImpacted = false;
		if (this.owningEntity != null)
		{
			Collider componentInChildren = base.GetComponentInChildren<Collider>();
			if (componentInChildren != null)
			{
				Collider[] componentsInChildren = this.owningEntity.gameObject.GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Physics.IgnoreCollision(componentInChildren, componentsInChildren[i]);
				}
			}
		}
	}

	// Token: 0x0600334C RID: 13132 RVA: 0x00118B64 File Offset: 0x00116D64
	private void Update()
	{
		if (this.entity.IsAuthority() && this.projectileHasImpacted && Time.timeAsDouble > this.projectileImpactTime + (double)this.postImpactLifetime)
		{
			this.entity.manager.RequestDestroyItem(this.entity.id);
		}
	}

	// Token: 0x0600334D RID: 13133 RVA: 0x00118BB8 File Offset: 0x00116DB8
	public void OnEntityInit()
	{
		this.owningEntityNetID = (int)this.entity.createData;
		if (this.owningEntityNetID != 0)
		{
			this.owningEntity = this.FindOwningEntity();
			this.projectileLauncher = this.owningEntity.GetComponent<IGameProjectileLauncher>();
			if (this.projectileLauncher != null)
			{
				this.projectileLauncher.OnProjectileInit(this);
			}
		}
	}

	// Token: 0x0600334E RID: 13134 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x0600334F RID: 13135 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06003350 RID: 13136 RVA: 0x00118C10 File Offset: 0x00116E10
	private GameEntity FindOwningEntity()
	{
		if (this.owningEntityNetID != 0)
		{
			GameEntityManager gameEntityManager = GhostReactorManager.Get(this.entity).gameEntityManager;
			GameEntityId entityIdFromNetId = gameEntityManager.GetEntityIdFromNetId(this.owningEntityNetID);
			return gameEntityManager.GetGameEntity(entityIdFromNetId);
		}
		return null;
	}

	// Token: 0x06003351 RID: 13137 RVA: 0x00118C4C File Offset: 0x00116E4C
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.projectileHasImpacted)
		{
			if (this.canHitPlayer)
			{
				Vector3 position = base.transform.position;
				if ((VRRig.LocalRig.GetMouthPosition() - position).sqrMagnitude < this.projectileHitRadius * this.projectileHitRadius && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
				{
					this.lastHitPlayerTime = Time.time;
					GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Ranged, this.entity.id, VRRig.LocalRig.GetComponent<GRPlayer>(), position);
				}
				if (this.projectileLauncher != null)
				{
					this.projectileLauncher.OnProjectileHit(this, collision);
				}
			}
			this.projectileHasImpacted = true;
			this.projectileImpactTime = Time.timeAsDouble;
		}
	}

	// Token: 0x06003352 RID: 13138 RVA: 0x00118D10 File Offset: 0x00116F10
	private void OnTriggerEnter(Collider collider)
	{
		if (!this.projectileHasImpacted)
		{
			GRShieldCollider component = collider.GetComponent<GRShieldCollider>();
			if (component != null)
			{
				component.BlockHittable(this.projectileRigidbody.transform.position, this.projectileRigidbody.linearVelocity.normalized, this.hittable);
			}
		}
	}

	// Token: 0x06003353 RID: 13139 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06003354 RID: 13140 RVA: 0x00118D64 File Offset: 0x00116F64
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		GRTool gameComponent = this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId);
		if (gameComponent != null)
		{
			switch (hitTypeId)
			{
			case GameHitType.Club:
				this.OnHitByClub(gameComponent, hit);
				return;
			case GameHitType.Flash:
				this.OnHitByFlash(gameComponent, hit);
				return;
			case GameHitType.Shield:
				this.OnHitByShield(gameComponent, hit);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06003355 RID: 13141 RVA: 0x00118DC8 File Offset: 0x00116FC8
	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		this.projectileHasImpacted = true;
		this.projectileImpactTime = Time.timeAsDouble;
		if (this.projectileRigidbody != null)
		{
			this.PlayImpactFX();
			this.projectileRigidbody.linearVelocity = hit.hitImpulse * (this.projectileRigidbody.linearVelocity.magnitude * 0.7f);
		}
	}

	// Token: 0x06003356 RID: 13142 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnHitByFlash(GRTool grTool, GameHitData hit)
	{
	}

	// Token: 0x06003357 RID: 13143 RVA: 0x00118E2A File Offset: 0x0011702A
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.projectileHasImpacted = true;
		this.projectileImpactTime = Time.timeAsDouble;
		if (this.projectileRigidbody != null)
		{
			this.PlayImpactFX();
			this.projectileRigidbody.linearVelocity = hit.hitImpulse;
		}
	}

	// Token: 0x06003358 RID: 13144 RVA: 0x00118E63 File Offset: 0x00117063
	private void PlayImpactFX()
	{
		if (this.particleSystem != null)
		{
			this.particleSystem.Play();
		}
		if (this.meshRenderer != null)
		{
			this.meshRenderer.enabled = false;
		}
	}

	// Token: 0x06003359 RID: 13145 RVA: 0x00118E98 File Offset: 0x00117098
	public void OnSuccessfulHit(GameHitData hit)
	{
		this.PlayImpactFX();
	}

	// Token: 0x0600335A RID: 13146 RVA: 0x00118EA0 File Offset: 0x001170A0
	public void OnSuccessfulHitPlayer(GRPlayer player, Vector3 hitPosition)
	{
		this.PlayImpactFX();
		this.hitSFX.Play(null);
		if (this.applyFreezeEffect)
		{
			player.SetAsFrozen(4f);
		}
	}

	// Token: 0x04004290 RID: 17040
	private int owningEntityNetID;

	// Token: 0x04004291 RID: 17041
	private GameEntity entity;

	// Token: 0x04004292 RID: 17042
	public GameEntity owningEntity;

	// Token: 0x04004293 RID: 17043
	private IGameProjectileLauncher projectileLauncher;

	// Token: 0x04004294 RID: 17044
	public Rigidbody projectileRigidbody;

	// Token: 0x04004295 RID: 17045
	private ParticleSystem particleSystem;

	// Token: 0x04004296 RID: 17046
	private AudioSource audioSource;

	// Token: 0x04004297 RID: 17047
	private MeshRenderer meshRenderer;

	// Token: 0x04004298 RID: 17048
	private GameHittable hittable;

	// Token: 0x04004299 RID: 17049
	public float projectileSpeed = 5f;

	// Token: 0x0400429A RID: 17050
	public float projectileHitRadius = 1f;

	// Token: 0x0400429B RID: 17051
	public float postImpactLifetime = 2f;

	// Token: 0x0400429C RID: 17052
	private bool projectileHasImpacted;

	// Token: 0x0400429D RID: 17053
	private double projectileImpactTime;

	// Token: 0x0400429E RID: 17054
	private float lastHitPlayerTime;

	// Token: 0x0400429F RID: 17055
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x040042A0 RID: 17056
	public bool applyFreezeEffect;

	// Token: 0x040042A1 RID: 17057
	public bool canHitPlayer = true;

	// Token: 0x040042A2 RID: 17058
	public AbilitySound hitSFX;
}
