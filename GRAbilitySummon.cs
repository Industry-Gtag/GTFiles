using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200074F RID: 1871
[Serializable]
public class GRAbilitySummon : GRAbilityBase
{
	// Token: 0x06002F8E RID: 12174 RVA: 0x00101FF0 File Offset: 0x001001F0
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002F8F RID: 12175 RVA: 0x00103390 File Offset: 0x00101590
	protected override void OnStart()
	{
		this.lastAnimIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.animData.Count, this.lastAnimIndex);
		this.duration = this.animData[this.lastAnimIndex].duration;
		this.chargeTime = this.animData[this.lastAnimIndex].eventTime;
		this.PlayAnim(this.animData[this.lastAnimIndex].animName, 0.1f, this.animSpeed);
		this.state = GRAbilitySummon.State.Charge;
		this.summonSound.Play(this.audioSource);
		this.spawnedCount = 0;
		this.agent.SetStopped(true);
		this.agent.SetSpeed(1f);
		if (this.fxStartSummon != null)
		{
			this.fxStartSummon.SetActive(false);
			this.fxStartSummon.SetActive(true);
		}
	}

	// Token: 0x06002F90 RID: 12176 RVA: 0x0010347A File Offset: 0x0010167A
	protected override void OnStop()
	{
		this.lookAtTarget = null;
		this.agent.SetStopped(false);
	}

	// Token: 0x06002F91 RID: 12177 RVA: 0x0010348F File Offset: 0x0010168F
	public void SetLookAtTarget(Transform transform)
	{
		this.lookAtTarget = transform;
	}

	// Token: 0x06002F92 RID: 12178 RVA: 0x00103498 File Offset: 0x00101698
	protected override void OnThink(float dt)
	{
		this.UpdateState(dt);
	}

	// Token: 0x06002F93 RID: 12179 RVA: 0x001034A1 File Offset: 0x001016A1
	protected override void OnUpdateShared(float dt)
	{
		if (this.lookAtTarget != null)
		{
			GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.lookAtTarget, 360f);
		}
	}

	// Token: 0x06002F94 RID: 12180 RVA: 0x001034D4 File Offset: 0x001016D4
	private void UpdateState(float dt)
	{
		double num = Time.timeAsDouble - this.startTime;
		switch (this.state)
		{
		case GRAbilitySummon.State.Charge:
			if (num > (double)this.chargeTime)
			{
				this.SetState(GRAbilitySummon.State.Spawn);
				return;
			}
			break;
		case GRAbilitySummon.State.Spawn:
			if (!this.spawned)
			{
				this.spawned = this.DoSpawn();
			}
			if (this.spawned && num > (double)this.duration)
			{
				this.SetState(GRAbilitySummon.State.Done);
				this.spawned = false;
			}
			break;
		case GRAbilitySummon.State.Done:
			break;
		default:
			return;
		}
	}

	// Token: 0x06002F95 RID: 12181 RVA: 0x0010354E File Offset: 0x0010174E
	private void SetState(GRAbilitySummon.State newState)
	{
		GRAbilitySummon.State state = this.state;
		this.state = newState;
		switch (newState)
		{
		default:
			return;
		}
	}

	// Token: 0x06002F96 RID: 12182 RVA: 0x00103570 File Offset: 0x00101770
	private Vector3? GetSpawnLocation()
	{
		if (this.summonMarkers != null && this.summonMarkers.Count > 0)
		{
			int num = Random.Range(0, this.summonMarkers.Count);
			if (this.summonMarkers[num] != null)
			{
				return new Vector3?(this.summonMarkers[num].transform.position);
			}
		}
		Vector3 position = this.root.position;
		float num2 = Random.Range(-this.summonConeAngle / 2f, this.summonConeAngle / 2f);
		int i = 0;
		while (i < 5)
		{
			Vector3 vector = Quaternion.Euler(0f, num2, 0f) * this.root.forward;
			Vector3 vector2 = position + vector * this.desiredSpawnDistance;
			NavMeshHit navMeshHit;
			if (!NavMesh.Raycast(position, vector2, out navMeshHit, this.walkableArea))
			{
				goto IL_0126;
			}
			if (navMeshHit.distance >= this.minSpawnDistance)
			{
				vector2 = navMeshHit.position + Vector3.up * this.spawnHeight;
				goto IL_0126;
			}
			num2 += 15f;
			if (num2 > this.summonConeAngle / 2f)
			{
				this.summonConeAngle = -this.summonConeAngle / 2f;
			}
			IL_0151:
			i++;
			continue;
			IL_0126:
			RaycastHit raycastHit;
			if (!Physics.Raycast(vector2, Vector3.down, out raycastHit) || raycastHit.collider.gameObject.GetComponent<GRHazardousMaterial>() == null)
			{
				return new Vector3?(vector2);
			}
			goto IL_0151;
		}
		return null;
	}

	// Token: 0x06002F97 RID: 12183 RVA: 0x001036E3 File Offset: 0x001018E3
	public bool ForceSpawn()
	{
		return this.DoSpawn();
	}

	// Token: 0x06002F98 RID: 12184 RVA: 0x001036EC File Offset: 0x001018EC
	private bool DoSpawn()
	{
		Vector3? spawnLocation = this.GetSpawnLocation();
		if (spawnLocation != null)
		{
			if (this.entity.IsAuthority())
			{
				Quaternion identity = Quaternion.identity;
				GhostReactorManager.Get(this.entity).gameEntityManager.RequestCreateItem(this.entityPrefabToSpawn.name.GetStaticHash(), spawnLocation.Value, identity, 0L, this.entity.id);
				this.spawnedCount++;
			}
			if (this.audioSource != null)
			{
				this.audioSource.PlayOneShot(this.summonSpawnAudioClip);
			}
			if (this.fxOnSpawn != null)
			{
				this.fxOnSpawn.SetActive(false);
				this.fxOnSpawn.SetActive(true);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06002F99 RID: 12185 RVA: 0x001037B0 File Offset: 0x001019B0
	public override bool IsDone()
	{
		return this.state == GRAbilitySummon.State.Done;
	}

	// Token: 0x06002F9A RID: 12186 RVA: 0x001037BB File Offset: 0x001019BB
	public override bool IsCoolDownOver()
	{
		return base.IsCoolDownOver(this.coolDown);
	}

	// Token: 0x06002F9B RID: 12187 RVA: 0x001037C9 File Offset: 0x001019C9
	public override float GetRange()
	{
		return this.range;
	}

	// Token: 0x04003CDD RID: 15581
	private int lastAnimIndex = -1;

	// Token: 0x04003CDE RID: 15582
	public GameEntity entityPrefabToSpawn;

	// Token: 0x04003CDF RID: 15583
	public List<AnimationData> animData;

	// Token: 0x04003CE0 RID: 15584
	private float animSpeed = 1f;

	// Token: 0x04003CE1 RID: 15585
	public float coolDown;

	// Token: 0x04003CE2 RID: 15586
	public float range;

	// Token: 0x04003CE3 RID: 15587
	public float chargeTime = 3f;

	// Token: 0x04003CE4 RID: 15588
	public float duration = 3f;

	// Token: 0x04003CE5 RID: 15589
	public float desiredSpawnDistance = 3f;

	// Token: 0x04003CE6 RID: 15590
	public float minSpawnDistance = 1f;

	// Token: 0x04003CE7 RID: 15591
	public float spawnHeight = 1f;

	// Token: 0x04003CE8 RID: 15592
	public float summonConeAngle = 120f;

	// Token: 0x04003CE9 RID: 15593
	private bool spawned;

	// Token: 0x04003CEA RID: 15594
	public AudioClip summonSpawnAudioClip;

	// Token: 0x04003CEB RID: 15595
	public GameObject fxStartSummon;

	// Token: 0x04003CEC RID: 15596
	public GameObject fxOnSpawn;

	// Token: 0x04003CED RID: 15597
	public AbilitySound summonSound;

	// Token: 0x04003CEE RID: 15598
	private int spawnedCount;

	// Token: 0x04003CEF RID: 15599
	public Transform lookAtTarget;

	// Token: 0x04003CF0 RID: 15600
	public List<GRAbilitySummon.SummonMarker> summonMarkers;

	// Token: 0x04003CF1 RID: 15601
	private GRAbilitySummon.State state;

	// Token: 0x02000750 RID: 1872
	[Serializable]
	public class SummonMarker
	{
		// Token: 0x04003CF2 RID: 15602
		public Transform transform;
	}

	// Token: 0x02000751 RID: 1873
	private enum State
	{
		// Token: 0x04003CF4 RID: 15604
		Charge,
		// Token: 0x04003CF5 RID: 15605
		Spawn,
		// Token: 0x04003CF6 RID: 15606
		Done
	}
}
