using System;
using System.Collections.Generic;
using GorillaTagScripts.GhostReactor;
using UnityEngine;

// Token: 0x0200073E RID: 1854
[Serializable]
public class GRAbilityDie : GRAbilityBase
{
	// Token: 0x06002F19 RID: 12057 RVA: 0x00100D0C File Offset: 0x000FEF0C
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		if (this.disableAllCollidersWhenDead)
		{
			agent.GetComponentsInChildren<Collider>(this.disableCollidersWhenDead);
		}
		if (this.disableAllRenderersWhenDead)
		{
			agent.GetComponentsInChildren<Renderer>(this.hideWhenDead);
		}
		GRAbilityDie.Disable(this.disableCollidersWhenDead, false);
		this.staggerMovement.Setup(root);
	}

	// Token: 0x06002F1A RID: 12058 RVA: 0x00100D6C File Offset: 0x000FEF6C
	protected override void OnStart()
	{
		this.totalDeathDelay = this.delayDeath;
		if (this.animData.Count > 0)
		{
			int num = Random.Range(0, this.animData.Count);
			this.totalDeathDelay += this.animData[num].duration;
			this.staggerMovement.InitFromVelocityAndDuration(this.staggerMovement.velocity, this.totalDeathDelay);
			this.PlayAnim(this.animData[num].animName, 0.1f, this.animData[num].speed);
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.isDead = false;
		if (this.doKnockback)
		{
			this.staggerMovement.Start();
		}
		this.soundDeath.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		this.soundOnHide.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		this.soundDeath.Play(null);
		GRAbilityDie.Disable(this.disableCollidersWhenDead, true);
		if (this.fxDeath != null)
		{
			this.fxDeath.SetActive(false);
		}
		this.events.Reset();
		this.events.OnAbilityStart(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
	}

	// Token: 0x06002F1B RID: 12059 RVA: 0x00100EB0 File Offset: 0x000FF0B0
	protected override void OnStop()
	{
		this.staggerMovement.Stop();
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		GRAbilityDie.Hide(this.hideWhenDead, false);
		GRAbilityDie.Disable(this.disableCollidersWhenDead, false);
		this.events.OnAbilityStop(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
	}

	// Token: 0x06002F1C RID: 12060 RVA: 0x00100F18 File Offset: 0x000FF118
	public void SetStaggerVelocity(Vector3 vel)
	{
		float magnitude = vel.magnitude;
		if (magnitude > 0f)
		{
			Vector3 vector = vel / magnitude;
			vector.y = 0f;
			vel = vector * magnitude;
		}
		this.staggerMovement.InitFromVelocityAndDuration(vel, this.totalDeathDelay);
	}

	// Token: 0x06002F1D RID: 12061 RVA: 0x00100F64 File Offset: 0x000FF164
	public void SetInstigatingPlayerIndex(int actorNumber)
	{
		Debug.Log(string.Format("SetInstigatingPlayerIndex {0}", actorNumber));
		this.instigatingActorNumber = actorNumber;
	}

	// Token: 0x06002F1E RID: 12062 RVA: 0x00100F84 File Offset: 0x000FF184
	private void Die()
	{
		this.soundOnHide.Play(null);
		if (this.fxDeath != null)
		{
			this.fxDeath.SetActive(false);
			this.fxDeath.SetActive(true);
		}
		GRAbilityDie.Hide(this.hideWhenDead, true);
		GRAbilityDie.Disable(this.disableCollidersWhenDead, true);
		GameEntity entity = this.agent.entity;
		GameEntity gameEntity;
		if (this.lootTable != null && entity.IsAuthority() && this.lootTable.TryForRandomItem(entity, out gameEntity, 0))
		{
			Transform transform = this.lootSpawnMarker;
			if (transform == null)
			{
				transform = this.agent.transform;
			}
			Vector3 vector = transform.position;
			if (transform == null)
			{
				vector.y += 0.33f;
			}
			RaycastHit raycastHit;
			if (this.spawnOnGround && Physics.Raycast(new Ray(vector + Vector3.up * 0.5f, -Vector3.up), out raycastHit, 5f, this.groundLayerMask.value, QueryTriggerInteraction.Ignore))
			{
				vector = raycastHit.point;
			}
			entity.manager.RequestCreateItem(gameEntity.gameObject.name.GetStaticHash(), vector, transform.rotation, 0L);
		}
		GREnemy component = entity.GetComponent<GREnemy>();
		if (component != null && component.damageFlash != null)
		{
			component.damageFlash.Play();
		}
	}

	// Token: 0x06002F1F RID: 12063 RVA: 0x001010F0 File Offset: 0x000FF2F0
	public void DestroySelf()
	{
		Debug.Log("DESTROY SELF");
		this.ReportDeathStat();
		if (this.agent.entity.IsAuthority())
		{
			this.agent.entity.manager.RequestDestroyItem(this.agent.entity.id);
		}
	}

	// Token: 0x06002F20 RID: 12064 RVA: 0x00101144 File Offset: 0x000FF344
	public void ReportDeathStat()
	{
		if (this.reported)
		{
			return;
		}
		this.reported = true;
		GameEntity entity = this.agent.entity;
		GRPlayer grplayer = GRPlayer.Get(this.instigatingActorNumber);
		if (grplayer != null)
		{
			grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.Kills, 1f);
		}
		GhostReactor.instance.shiftManager.shiftStats.IncrementEnemyKills(entity.GetEnemyType());
	}

	// Token: 0x06002F21 RID: 12065 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsDone()
	{
		return false;
	}

	// Token: 0x06002F22 RID: 12066 RVA: 0x001011A8 File Offset: 0x000FF3A8
	protected override void OnUpdateShared(float dt)
	{
		if (this.startTime >= 0.0)
		{
			if (this.doKnockback)
			{
				this.staggerMovement.Update(dt);
			}
			double num = Time.timeAsDouble - this.startTime;
			if (!this.isDead && num > (double)this.totalDeathDelay)
			{
				this.isDead = true;
				this.Die();
			}
			else if (this.isDead && num > (double)(this.totalDeathDelay + this.destroyDelay))
			{
				GhostReactorManager.Get(this.entity).OnAbilityDie(this.entity, this.delayRespawn);
				this.DestroySelf();
				this.startTime = -1.0;
			}
			this.events.TryPlay((float)num, this.audioSource);
		}
	}

	// Token: 0x06002F23 RID: 12067 RVA: 0x00101268 File Offset: 0x000FF468
	public static void Hide(List<Renderer> renderers, bool hide)
	{
		if (renderers == null)
		{
			return;
		}
		for (int i = 0; i < renderers.Count; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x06002F24 RID: 12068 RVA: 0x001012AC File Offset: 0x000FF4AC
	public static void Disable(List<Collider> colliders, bool disable)
	{
		if (colliders == null)
		{
			return;
		}
		for (int i = 0; i < colliders.Count; i++)
		{
			if (colliders[i] != null)
			{
				colliders[i].enabled = !disable;
			}
		}
	}

	// Token: 0x04003C40 RID: 15424
	public float delayDeath;

	// Token: 0x04003C41 RID: 15425
	public float delayRespawn = -1f;

	// Token: 0x04003C42 RID: 15426
	public List<Renderer> hideWhenDead;

	// Token: 0x04003C43 RID: 15427
	public List<Collider> disableCollidersWhenDead;

	// Token: 0x04003C44 RID: 15428
	public bool disableAllCollidersWhenDead;

	// Token: 0x04003C45 RID: 15429
	public bool disableAllRenderersWhenDead;

	// Token: 0x04003C46 RID: 15430
	public GameObject fxDeath;

	// Token: 0x04003C47 RID: 15431
	public AbilitySound soundDeath;

	// Token: 0x04003C48 RID: 15432
	public AbilitySound soundOnHide;

	// Token: 0x04003C49 RID: 15433
	public float destroyDelay = 3f;

	// Token: 0x04003C4A RID: 15434
	public bool doKnockback = true;

	// Token: 0x04003C4B RID: 15435
	public GRBreakableItemSpawnConfig lootTable;

	// Token: 0x04003C4C RID: 15436
	public bool spawnOnGround;

	// Token: 0x04003C4D RID: 15437
	public LayerMask groundLayerMask;

	// Token: 0x04003C4E RID: 15438
	public Transform lootSpawnMarker;

	// Token: 0x04003C4F RID: 15439
	public List<AnimationData> animData;

	// Token: 0x04003C50 RID: 15440
	private int instigatingActorNumber;

	// Token: 0x04003C51 RID: 15441
	private bool isDead;

	// Token: 0x04003C52 RID: 15442
	private float totalDeathDelay;

	// Token: 0x04003C53 RID: 15443
	public GRAbilityInterpolatedMovement staggerMovement;

	// Token: 0x04003C54 RID: 15444
	public GameAbilityEvents events;

	// Token: 0x04003C55 RID: 15445
	private bool reported;
}
