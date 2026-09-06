using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CjLib;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

// Token: 0x0200079E RID: 1950
public class GREnemyMonkeye : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent
{
	// Token: 0x060031A1 RID: 12705 RVA: 0x0010DF88 File Offset: 0x0010C188
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x060031A2 RID: 12706 RVA: 0x0010E010 File Offset: 0x0010C210
	public void OnEntityInit()
	{
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource, null, null, null);
		this.abilityChase.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilitySearch.Setup(this.agent, this.anim, this.audioSource, null, null, null);
		this.abilityAttackLaser.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityAttackDiscoWander.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityAttackSlamdown.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityPatrol.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.senseNearby.Setup(this.headTransform, this.entity);
		this.Setup(this.entity.createData);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry grbonusEntry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(grbonusEntry);
			}
		}
		this.agent.navAgent.autoTraverseOffMeshLink = false;
		this.agent.onJumpRequested += this.OnAgentJumpRequested;
	}

	// Token: 0x060031A3 RID: 12707 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060031A4 RID: 12708 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060031A5 RID: 12709 RVA: 0x0010E2D0 File Offset: 0x0010C4D0
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x060031A6 RID: 12710 RVA: 0x0010E300 File Offset: 0x0010C500
	public void Setup(long entityCreateData)
	{
		this.SetPatrolPath(entityCreateData);
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyMonkeye.Behavior.Patrol, true);
		}
		else
		{
			this.SetBehavior(GREnemyMonkeye.Behavior.Idle, true);
		}
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyMonkeye.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyMonkeye.BodyState.Bones, true);
	}

	// Token: 0x060031A7 RID: 12711 RVA: 0x0010E353 File Offset: 0x0010C553
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyMonkeye.Behavior.Jump, false);
	}

	// Token: 0x060031A8 RID: 12712 RVA: 0x0010E36E File Offset: 0x0010C56E
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 11)
		{
			return;
		}
		this.SetBehavior((GREnemyMonkeye.Behavior)newState, false);
	}

	// Token: 0x060031A9 RID: 12713 RVA: 0x0010E382 File Offset: 0x0010C582
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyMonkeye.BodyState)newState, false);
	}

	// Token: 0x060031AA RID: 12714 RVA: 0x0010E398 File Offset: 0x0010C598
	public void SetPatrolPath(long entityCreateData)
	{
		GRPatrolPath grpatrolPath = GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(entityCreateData);
		this.abilityPatrol.SetPatrolPath(grpatrolPath);
	}

	// Token: 0x060031AB RID: 12715 RVA: 0x0010E3C8 File Offset: 0x0010C5C8
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x060031AC RID: 12716 RVA: 0x0010E3D1 File Offset: 0x0010C5D1
	public bool TrySetBehavior(GREnemyMonkeye.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemyMonkeye.Behavior.Jump && newBehavior == GREnemyMonkeye.Behavior.Stagger)
		{
			return false;
		}
		if (newBehavior == GREnemyMonkeye.Behavior.Stagger && Time.time < this.lastStaggerTime + this.staggerImmuneTime)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x060031AD RID: 12717 RVA: 0x0010E408 File Offset: 0x0010C608
	public void SetBehavior(GREnemyMonkeye.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemyMonkeye.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyMonkeye.Behavior.Patrol:
			this.abilityPatrol.Stop();
			break;
		case GREnemyMonkeye.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyMonkeye.Behavior.Dying:
			this.abilityDie.Stop();
			break;
		case GREnemyMonkeye.Behavior.Chase:
			this.abilityChase.Stop();
			break;
		case GREnemyMonkeye.Behavior.Search:
			this.abilitySearch.Stop();
			break;
		case GREnemyMonkeye.Behavior.Attack:
			this.abilityAttackLaser.Stop();
			break;
		case GREnemyMonkeye.Behavior.AttackDisco:
			this.abilityAttackDiscoWander.Stop();
			break;
		case GREnemyMonkeye.Behavior.AttackSlamdown:
			this.abilityAttackSlamdown.Stop();
			break;
		case GREnemyMonkeye.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyMonkeye.Behavior.Jump:
			this.abilityJump.Stop();
			this.lastJumpEndtime = Time.timeAsDouble;
			break;
		}
		this.currBehavior = newBehavior;
		switch (this.currBehavior)
		{
		case GREnemyMonkeye.Behavior.Idle:
			this.abilitySearch.Start();
			break;
		case GREnemyMonkeye.Behavior.Patrol:
			this.abilityPatrol.Start();
			break;
		case GREnemyMonkeye.Behavior.Stagger:
			this.abilityStagger.Start();
			this.lastStaggerTime = Time.time;
			break;
		case GREnemyMonkeye.Behavior.Dying:
			this.abilityDie.Start();
			break;
		case GREnemyMonkeye.Behavior.Chase:
			this.abilityChase.Start();
			this.investigateLocation = null;
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyMonkeye.Behavior.Search:
			this.abilitySearch.Start();
			break;
		case GREnemyMonkeye.Behavior.Attack:
			this.abilityAttackLaser.Start();
			this.investigateLocation = null;
			this.abilityAttackLaser.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyMonkeye.Behavior.AttackDisco:
			this.abilityAttackDiscoWander.Start();
			this.investigateLocation = null;
			break;
		case GREnemyMonkeye.Behavior.AttackSlamdown:
			this.abilityAttackSlamdown.Start();
			this.investigateLocation = null;
			this.abilityAttackSlamdown.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyMonkeye.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyMonkeye.Behavior.Jump:
			this.abilityJump.Start();
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x060031AE RID: 12718 RVA: 0x0010E680 File Offset: 0x0010C880
	private int CalcMaxHP()
	{
		float difficultyScalingForCurrentFloor = this.entity.manager.ghostReactorManager.reactor.difficultyScalingForCurrentFloor;
		return (int)((float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax) * difficultyScalingForCurrentFloor);
	}

	// Token: 0x060031AF RID: 12719 RVA: 0x0010E6BC File Offset: 0x0010C8BC
	public void SetBodyState(GREnemyMonkeye.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyMonkeye.BodyState.Bones:
			this.hp = this.CalcMaxHP();
			this.enemy.SetMaxHP(this.hp);
			this.enemy.SetHP(this.hp);
			break;
		case GREnemyMonkeye.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyMonkeye.BodyState.Destroyed:
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyMonkeye.BodyState.Bones:
			this.hp = this.CalcMaxHP();
			this.enemy.SetMaxHP(this.hp);
			this.enemy.SetHP(this.hp);
			break;
		case GREnemyMonkeye.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x060031B0 RID: 12720 RVA: 0x0010E7CC File Offset: 0x0010C9CC
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyMonkeye.BodyState.Destroyed:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyMonkeye.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyMonkeye.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x060031B1 RID: 12721 RVA: 0x0010E866 File Offset: 0x0010CA66
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x060031B2 RID: 12722 RVA: 0x0010E874 File Offset: 0x0010CA74
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyMonkeye.tempRigs.Clear();
		GREnemyMonkeye.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyMonkeye.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyMonkeye.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemyMonkeye.Behavior.Idle:
		case GREnemyMonkeye.Behavior.Patrol:
		case GREnemyMonkeye.Behavior.Investigate:
			this.ChooseNewBehavior();
			return;
		case GREnemyMonkeye.Behavior.Stagger:
		case GREnemyMonkeye.Behavior.Dying:
		case GREnemyMonkeye.Behavior.Attack:
		case GREnemyMonkeye.Behavior.AttackSlamdown:
			break;
		case GREnemyMonkeye.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemyMonkeye.Behavior.Search:
			this.ChooseNewBehavior();
			return;
		case GREnemyMonkeye.Behavior.AttackDisco:
			this.abilityAttackDiscoWander.Think(dt);
			break;
		default:
			return;
		}
	}

	// Token: 0x060031B3 RID: 12723 RVA: 0x0010E980 File Offset: 0x0010CB80
	private bool TryChooseAttackBehavior(float toPlayerDistSq)
	{
		if (toPlayerDistSq < this.abilityAttackLaser.GetRange() * this.abilityAttackLaser.GetRange() && this.abilityAttackLaser.IsCoolDownOver())
		{
			this.SetBehavior(GREnemyMonkeye.Behavior.Attack, false);
			return true;
		}
		if (this.senseNearby.IsAnyoneNearby(this.abilityAttackDiscoWander.GetRange(), false) && this.abilityAttackDiscoWander.IsCoolDownOver())
		{
			this.SetBehavior(GREnemyMonkeye.Behavior.AttackDisco, false);
			return true;
		}
		if (this.senseNearby.IsAnyoneNearby(this.abilityAttackSlamdown.GetRange(), false) && this.abilityAttackSlamdown.IsCoolDownOver())
		{
			this.SetBehavior(GREnemyMonkeye.Behavior.AttackSlamdown, false);
			return true;
		}
		return false;
	}

	// Token: 0x060031B4 RID: 12724 RVA: 0x0010EA24 File Offset: 0x0010CC24
	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
		{
			if (this.agent.targetPlayer != null)
			{
				Vector3 position = GRPlayer.Get(this.agent.targetPlayer).transform.position;
				Vector3 vector = position - base.transform.position;
				float magnitude = vector.magnitude;
				if (this.TryChooseAttackBehavior(magnitude * magnitude))
				{
					return;
				}
				if (this.canChaseJump && this.abilityJump.IsCoolDownOver(this.chaseJumpMinInterval) && magnitude > this.attackRange + this.minChaseJumpDistance && GRSenseLineOfSight.HasNavmeshLineOfSight(base.transform.position, position, 10f))
				{
					Vector3 vector2 = vector / magnitude;
					float num = Mathf.Clamp(this.chaseJumpDistance, this.minChaseJumpDistance, magnitude - this.attackRange * 0.5f);
					NavMeshHit navMeshHit;
					if (NavMesh.SamplePosition(base.transform.position + vector2 * num, out navMeshHit, 0.5f, AbilityHelperFunctions.GetNavMeshWalkableArea()))
					{
						this.agent.GetGameAgentManager().RequestJump(this.agent, base.transform.position, navMeshHit.position, 0.25f, 1.5f);
						return;
					}
				}
			}
			if (!this.abilityAttackLaser.IsCoolDownOver())
			{
				this.TrySetBehavior(GREnemyMonkeye.Behavior.Idle);
				return;
			}
			this.TrySetBehavior(GREnemyMonkeye.Behavior.Chase);
			return;
		}
		else
		{
			this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
			if (this.investigateLocation != null)
			{
				this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
				this.SetBehavior(GREnemyMonkeye.Behavior.Investigate, false);
				return;
			}
			if (this.abilityPatrol.HasValidPatrolPath())
			{
				this.SetBehavior(GREnemyMonkeye.Behavior.Patrol, false);
				return;
			}
			this.SetBehavior(GREnemyMonkeye.Behavior.Idle, false);
			return;
		}
	}

	// Token: 0x060031B5 RID: 12725 RVA: 0x0010EBFE File Offset: 0x0010CDFE
	private void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x060031B6 RID: 12726 RVA: 0x0010EC1C File Offset: 0x0010CE1C
	private void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyMonkeye.Behavior.Idle:
			this.abilityIdle.UpdateAuthority(dt);
			return;
		case GREnemyMonkeye.Behavior.Patrol:
			this.abilityPatrol.UpdateAuthority(dt);
			return;
		case GREnemyMonkeye.Behavior.Stagger:
			this.abilityStagger.UpdateAuthority(dt);
			if (this.abilityStagger.IsDone())
			{
				if (this.agent.targetPlayer == null)
				{
					this.SetBehavior(GREnemyMonkeye.Behavior.Search, false);
					return;
				}
				this.SetBehavior(GREnemyMonkeye.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.Dying:
			this.abilityDie.UpdateAuthority(dt);
			return;
		case GREnemyMonkeye.Behavior.Chase:
		{
			this.abilityChase.UpdateAuthority(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyMonkeye.Behavior.Search, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float sqrMagnitude = (grplayer.transform.position - base.transform.position).sqrMagnitude;
				this.TryChooseAttackBehavior(sqrMagnitude);
				return;
			}
			break;
		}
		case GREnemyMonkeye.Behavior.Search:
			this.abilitySearch.UpdateAuthority(dt);
			if (this.abilitySearch.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.Attack:
			this.abilityAttackLaser.UpdateAuthority(dt);
			if (this.abilityAttackLaser.IsDone())
			{
				this.SetBehavior(GREnemyMonkeye.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.AttackDisco:
			this.abilityAttackDiscoWander.UpdateAuthority(dt);
			if (this.abilityAttackDiscoWander.IsDone())
			{
				this.SetBehavior(GREnemyMonkeye.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.AttackSlamdown:
			this.abilityAttackSlamdown.UpdateAuthority(dt);
			if (this.abilityAttackSlamdown.IsDone())
			{
				this.SetBehavior(GREnemyMonkeye.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.Investigate:
			this.abilityInvestigate.UpdateAuthority(dt);
			if (this.abilityInvestigate.IsDone())
			{
				this.investigateLocation = null;
			}
			if (GhostReactorManager.noiseDebugEnabled)
			{
				DebugUtil.DrawLine(base.transform.position, this.abilityInvestigate.GetTargetPos(), Color.green, true);
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.Jump:
			this.abilityJump.UpdateAuthority(dt);
			if (this.abilityJump.IsDone())
			{
				this.ChooseNewBehavior();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060031B7 RID: 12727 RVA: 0x0010EE30 File Offset: 0x0010D030
	private void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyMonkeye.Behavior.Idle:
			this.abilityIdle.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Patrol:
			this.abilityPatrol.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Dying:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Chase:
			this.abilityChase.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Search:
			this.abilitySearch.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Attack:
			this.abilityAttackLaser.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.AttackDisco:
			this.abilityAttackDiscoWander.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.AttackSlamdown:
			this.abilityAttackSlamdown.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			return;
		default:
			return;
		}
	}

	// Token: 0x060031B8 RID: 12728 RVA: 0x0010EF08 File Offset: 0x0010D108
	private void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState == GREnemyMonkeye.BodyState.Bones)
		{
			this.hp -= hit.hitAmount;
			this.enemy.SetHP(this.hp);
			if (this.damagedSounds.Count > 0)
			{
				this.damagedSoundIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.damagedSounds.Count, this.damagedSoundIndex);
				this.audioSource.PlayOneShot(this.damagedSounds[this.damagedSoundIndex], this.damagedSoundVolume);
			}
			if (this.fxDamaged != null)
			{
				this.fxDamaged.SetActive(false);
				this.fxDamaged.SetActive(true);
			}
			if (this.hp <= 0)
			{
				this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hit.hitByEntityId));
				this.SetBodyState(GREnemyMonkeye.BodyState.Destroyed, false);
				this.SetBehavior(GREnemyMonkeye.Behavior.Dying, false);
				return;
			}
			this.lastSeenTargetPosition = tool.transform.position;
			this.lastSeenTargetTime = Time.timeAsDouble;
			Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
			vector.y = 0f;
			this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
			if (this.allowStagger)
			{
				this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
				this.TrySetBehavior(GREnemyMonkeye.Behavior.Stagger);
				return;
			}
		}
		else if (this.currBodyState == GREnemyMonkeye.BodyState.Shell && this.armor != null)
		{
			this.armor.PlayBlockFx(hit.hitEntityPosition);
		}
	}

	// Token: 0x060031B9 RID: 12729 RVA: 0x0010F099 File Offset: 0x0010D299
	public void InstantDeath()
	{
		this.hp = 0;
		this.SetBodyState(GREnemyMonkeye.BodyState.Destroyed, false);
		this.SetBehavior(GREnemyMonkeye.Behavior.Dying, false);
	}

	// Token: 0x060031BA RID: 12730 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnHitByFlash(GRTool grTool, GameHitData hit)
	{
	}

	// Token: 0x060031BB RID: 12731 RVA: 0x0010F0B2 File Offset: 0x0010D2B2
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	// Token: 0x060031BC RID: 12732 RVA: 0x0010F0BC File Offset: 0x0010D2BC
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyMonkeye.BodyState.Destroyed)
		{
			return;
		}
		if (this.currBehavior != GREnemyMonkeye.Behavior.Attack && this.currBehavior != GREnemyMonkeye.Behavior.AttackDisco && this.currBehavior != GREnemyMonkeye.Behavior.AttackSlamdown)
		{
			return;
		}
		GRShieldCollider component = collider.GetComponent<GRShieldCollider>();
		if (component != null)
		{
			GameHittable component2 = base.GetComponent<GameHittable>();
			component.BlockHittable(this.headTransform.position, base.transform.forward, component2);
			return;
		}
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			GRPlayer component3 = attachedRigidbody.GetComponent<GRPlayer>();
			if (component3 != null && component3.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
			{
				if (this.tryHitPlayerCoroutine != null)
				{
					base.StopCoroutine(this.tryHitPlayerCoroutine);
				}
				this.tryHitPlayerCoroutine = base.StartCoroutine(this.TryHitPlayer(component3));
			}
			GRBreakable component4 = attachedRigidbody.GetComponent<GRBreakable>();
			GameHittable component5 = attachedRigidbody.GetComponent<GameHittable>();
			if (component4 != null && component5 != null)
			{
				GameHitData gameHitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = component5.gameEntity.id,
					hitByEntityId = this.entity.id,
					hitEntityPosition = component4.transform.position,
					hitImpulse = Vector3.zero,
					hitPosition = component4.transform.position,
					hittablePoint = component5.FindHittablePoint(collider)
				};
				component5.RequestHit(gameHitData);
			}
		}
	}

	// Token: 0x060031BD RID: 12733 RVA: 0x0010F23B File Offset: 0x0010D43B
	private IEnumerator TryHitPlayer(GRPlayer player)
	{
		yield return new WaitForUpdate();
		if ((this.currBehavior == GREnemyMonkeye.Behavior.Attack || this.currBehavior == GREnemyMonkeye.Behavior.AttackDisco || this.currBehavior == GREnemyMonkeye.Behavior.AttackSlamdown) && player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			Vector3 vector = player.transform.position - base.transform.position;
			vector.y = 0f;
			vector = vector.normalized * 6f;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position, vector);
		}
		yield break;
	}

	// Token: 0x060031BE RID: 12734 RVA: 0x0010F254 File Offset: 0x0010D454
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		strings.Add(string.Format("speed: <color=\"yellow\">{0}<color=\"white\"> patrol node:<color=\"yellow\">{1}/{2}<color=\"white\">", this.navAgent.speed, this.abilityPatrol.nextPatrolNode, (this.abilityPatrol.GetPatrolPath() != null) ? this.abilityPatrol.GetPatrolPath().patrolNodes.Count : 0));
	}

	// Token: 0x060031BF RID: 12735 RVA: 0x0010F2F8 File Offset: 0x0010D4F8
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte b = (byte)this.currBehavior;
		byte b2 = (byte)this.currBodyState;
		byte b3 = (byte)this.abilityPatrol.nextPatrolNode;
		int num = ((this.targetPlayer == null) ? (-1) : this.targetPlayer.ActorNumber);
		writer.Write(b);
		writer.Write(b2);
		writer.Write(this.hp);
		writer.Write(b3);
		writer.Write(num);
	}

	// Token: 0x060031C0 RID: 12736 RVA: 0x0010F364 File Offset: 0x0010D564
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyMonkeye.Behavior behavior = (GREnemyMonkeye.Behavior)reader.ReadByte();
		GREnemyMonkeye.BodyState bodyState = (GREnemyMonkeye.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		byte b = reader.ReadByte();
		int num2 = reader.ReadInt32();
		this.SetPatrolPath(this.entity.createData);
		this.abilityPatrol.SetNextPatrolNode((int)b);
		this.SetHP(num);
		this.SetBehavior(behavior, true);
		this.SetBodyState(bodyState, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(num2);
	}

	// Token: 0x060031C1 RID: 12737 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x060031C2 RID: 12738 RVA: 0x0010F3DC File Offset: 0x0010D5DC
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
				break;
			case GameHitType.Flash:
				this.OnHitByFlash(gameComponent, hit);
				break;
			case GameHitType.Shield:
				this.OnHitByShield(gameComponent, hit);
				break;
			}
			if (gameComponent.gameEntity != null)
			{
				this.senseNearby.OnHitByPlayer(gameComponent.gameEntity.lastHeldByActorNumber);
			}
		}
	}

	// Token: 0x04003FD0 RID: 16336
	public GameEntity entity;

	// Token: 0x04003FD1 RID: 16337
	public GameAgent agent;

	// Token: 0x04003FD2 RID: 16338
	public GREnemy enemy;

	// Token: 0x04003FD3 RID: 16339
	public GRArmorEnemy armor;

	// Token: 0x04003FD4 RID: 16340
	public GameHittable hittable;

	// Token: 0x04003FD5 RID: 16341
	[SerializeField]
	private GRAttributes attributes;

	// Token: 0x04003FD6 RID: 16342
	public GRSenseNearby senseNearby;

	// Token: 0x04003FD7 RID: 16343
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003FD8 RID: 16344
	public Animation anim;

	// Token: 0x04003FD9 RID: 16345
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003FDA RID: 16346
	public GRAbilityChase abilityChase;

	// Token: 0x04003FDB RID: 16347
	public GRAbilityIdle abilitySearch;

	// Token: 0x04003FDC RID: 16348
	[FormerlySerializedAs("abilityAttackSwipe")]
	public GRAbilityAttackLaser abilityAttackLaser;

	// Token: 0x04003FDD RID: 16349
	public GRAbilityAttackSimpleWander abilityAttackDiscoWander;

	// Token: 0x04003FDE RID: 16350
	public GRAbilityAttackSimple abilityAttackSlamdown;

	// Token: 0x04003FDF RID: 16351
	public bool allowStagger;

	// Token: 0x04003FE0 RID: 16352
	public GRAbilityStagger abilityStagger;

	// Token: 0x04003FE1 RID: 16353
	public GRAbilityDie abilityDie;

	// Token: 0x04003FE2 RID: 16354
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x04003FE3 RID: 16355
	public GRAbilityPatrol abilityPatrol;

	// Token: 0x04003FE4 RID: 16356
	public GRAbilityJump abilityJump;

	// Token: 0x04003FE5 RID: 16357
	public List<Renderer> bones;

	// Token: 0x04003FE6 RID: 16358
	public List<Renderer> always;

	// Token: 0x04003FE7 RID: 16359
	public Transform headTransform;

	// Token: 0x04003FE8 RID: 16360
	public float turnSpeed = 540f;

	// Token: 0x04003FE9 RID: 16361
	public float attackRange = 1.5f;

	// Token: 0x04003FEA RID: 16362
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x04003FEB RID: 16363
	public NavMeshAgent navAgent;

	// Token: 0x04003FEC RID: 16364
	public AudioSource audioSource;

	// Token: 0x04003FED RID: 16365
	public AudioClip damagedSound;

	// Token: 0x04003FEE RID: 16366
	public float damagedSoundVolume;

	// Token: 0x04003FEF RID: 16367
	public List<AudioClip> damagedSounds;

	// Token: 0x04003FF0 RID: 16368
	private int damagedSoundIndex;

	// Token: 0x04003FF1 RID: 16369
	public GameObject fxDamaged;

	// Token: 0x04003FF2 RID: 16370
	private Vector3? investigateLocation;

	// Token: 0x04003FF3 RID: 16371
	private float lastStaggerTime;

	// Token: 0x04003FF4 RID: 16372
	public float staggerImmuneTime = 10f;

	// Token: 0x04003FF5 RID: 16373
	private Transform target;

	// Token: 0x04003FF6 RID: 16374
	[ReadOnly]
	public int hp;

	// Token: 0x04003FF7 RID: 16375
	[ReadOnly]
	public GREnemyMonkeye.Behavior currBehavior;

	// Token: 0x04003FF8 RID: 16376
	[ReadOnly]
	public GREnemyMonkeye.BodyState currBodyState;

	// Token: 0x04003FF9 RID: 16377
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04003FFA RID: 16378
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x04003FFB RID: 16379
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x04003FFC RID: 16380
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x04003FFD RID: 16381
	private double lastJumpEndtime;

	// Token: 0x04003FFE RID: 16382
	public bool canChaseJump = true;

	// Token: 0x04003FFF RID: 16383
	public float chaseJumpDistance = 5f;

	// Token: 0x04004000 RID: 16384
	public float chaseJumpMinInterval = 1f;

	// Token: 0x04004001 RID: 16385
	public float minChaseJumpDistance = 2f;

	// Token: 0x04004002 RID: 16386
	private Rigidbody rigidBody;

	// Token: 0x04004003 RID: 16387
	private List<Collider> colliders;

	// Token: 0x04004004 RID: 16388
	private float lastHitPlayerTime;

	// Token: 0x04004005 RID: 16389
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x04004006 RID: 16390
	public float hearingRadius = 5f;

	// Token: 0x04004007 RID: 16391
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x04004008 RID: 16392
	private Coroutine tryHitPlayerCoroutine;

	// Token: 0x0200079F RID: 1951
	public enum Behavior
	{
		// Token: 0x0400400A RID: 16394
		Idle,
		// Token: 0x0400400B RID: 16395
		Patrol,
		// Token: 0x0400400C RID: 16396
		Stagger,
		// Token: 0x0400400D RID: 16397
		Dying,
		// Token: 0x0400400E RID: 16398
		Chase,
		// Token: 0x0400400F RID: 16399
		Search,
		// Token: 0x04004010 RID: 16400
		Attack,
		// Token: 0x04004011 RID: 16401
		AttackDisco,
		// Token: 0x04004012 RID: 16402
		AttackSlamdown,
		// Token: 0x04004013 RID: 16403
		Investigate,
		// Token: 0x04004014 RID: 16404
		Jump,
		// Token: 0x04004015 RID: 16405
		Count
	}

	// Token: 0x020007A0 RID: 1952
	public enum BodyState
	{
		// Token: 0x04004017 RID: 16407
		Destroyed,
		// Token: 0x04004018 RID: 16408
		Bones,
		// Token: 0x04004019 RID: 16409
		Shell,
		// Token: 0x0400401A RID: 16410
		Count
	}
}
