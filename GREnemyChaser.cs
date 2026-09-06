using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CjLib;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000799 RID: 1945
public class GREnemyChaser : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent
{
	// Token: 0x06003175 RID: 12661 RVA: 0x0010C7E8 File Offset: 0x0010A9E8
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

	// Token: 0x06003176 RID: 12662 RVA: 0x0010C870 File Offset: 0x0010AA70
	public void OnEntityInit()
	{
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource, null, null, null);
		this.abilityChase.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilitySearch.Setup(this.agent, this.anim, this.audioSource, null, null, null);
		this.abilityAttackSwipe.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityPatrol.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityWander.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityFlashed.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
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

	// Token: 0x06003177 RID: 12663 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06003178 RID: 12664 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06003179 RID: 12665 RVA: 0x0010CB20 File Offset: 0x0010AD20
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x0600317A RID: 12666 RVA: 0x0010CB50 File Offset: 0x0010AD50
	private void Setup(long entityCreateData)
	{
		this.SetPatrolPath(entityCreateData);
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyChaser.Behavior.Patrol, true);
		}
		else
		{
			this.SetBehavior(GREnemyChaser.Behavior.Wander, true);
		}
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyChaser.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyChaser.BodyState.Bones, true);
	}

	// Token: 0x0600317B RID: 12667 RVA: 0x0010CBA3 File Offset: 0x0010ADA3
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyChaser.Behavior.Jump, false);
	}

	// Token: 0x0600317C RID: 12668 RVA: 0x0010CBBE File Offset: 0x0010ADBE
	private void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 11)
		{
			return;
		}
		this.SetBehavior((GREnemyChaser.Behavior)newState, false);
	}

	// Token: 0x0600317D RID: 12669 RVA: 0x0010CBD2 File Offset: 0x0010ADD2
	private void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyChaser.BodyState)newState, false);
	}

	// Token: 0x0600317E RID: 12670 RVA: 0x0010CBE8 File Offset: 0x0010ADE8
	private void SetPatrolPath(long entityCreateData)
	{
		GRPatrolPath grpatrolPath = GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(entityCreateData);
		this.abilityPatrol.SetPatrolPath(grpatrolPath);
	}

	// Token: 0x0600317F RID: 12671 RVA: 0x0010CC18 File Offset: 0x0010AE18
	private void SetNextPatrolNode(int nextPatrolNode)
	{
		this.abilityPatrol.SetNextPatrolNode(nextPatrolNode);
	}

	// Token: 0x06003180 RID: 12672 RVA: 0x0010CC26 File Offset: 0x0010AE26
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x06003181 RID: 12673 RVA: 0x0010CC2F File Offset: 0x0010AE2F
	private bool TrySetBehavior(GREnemyChaser.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemyChaser.Behavior.Jump && newBehavior == GREnemyChaser.Behavior.Stagger)
		{
			return false;
		}
		if (newBehavior == GREnemyChaser.Behavior.Stagger && Time.time < this.lastStaggerTime + this.staggerImmuneTime)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x06003182 RID: 12674 RVA: 0x0010CC64 File Offset: 0x0010AE64
	private void SetBehavior(GREnemyChaser.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.Stop();
			break;
		case GREnemyChaser.Behavior.Wander:
			this.abilityWander.Stop();
			break;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyChaser.Behavior.Dying:
			this.abilityDie.Stop();
			break;
		case GREnemyChaser.Behavior.Chase:
			this.abilityChase.Stop();
			break;
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.Stop();
			break;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.Stop();
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.Stop();
			break;
		case GREnemyChaser.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyChaser.Behavior.Jump:
			this.abilityJump.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilitySearch.Start();
			break;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.Start();
			break;
		case GREnemyChaser.Behavior.Wander:
			this.abilityWander.Start();
			break;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Start();
			this.lastStaggerTime = Time.time;
			break;
		case GREnemyChaser.Behavior.Dying:
			if (this.entity.IsAuthority())
			{
				this.entity.manager.RequestCreateItem(this.corePrefab.gameObject.name.GetStaticHash(), this.coreMarker.position, this.coreMarker.rotation, 0L);
			}
			this.abilityDie.Start();
			break;
		case GREnemyChaser.Behavior.Chase:
			this.abilityChase.Start();
			this.investigateLocation = null;
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.Start();
			break;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.Start();
			this.investigateLocation = null;
			this.abilityAttackSwipe.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.Start();
			break;
		case GREnemyChaser.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyChaser.Behavior.Jump:
			this.abilityJump.Start();
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x06003183 RID: 12675 RVA: 0x0010CEEF File Offset: 0x0010B0EF
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null)
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x06003184 RID: 12676 RVA: 0x0010CF20 File Offset: 0x0010B120
	private void SetBodyState(GREnemyChaser.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyChaser.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Destroyed:
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyChaser.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyChaser.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x06003185 RID: 12677 RVA: 0x0010CFF8 File Offset: 0x0010B1F8
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Destroyed:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyChaser.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyChaser.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003186 RID: 12678 RVA: 0x0010D092 File Offset: 0x0010B292
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x06003187 RID: 12679 RVA: 0x0010D0A0 File Offset: 0x0010B2A0
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyChaser.tempRigs.Clear();
		GREnemyChaser.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyChaser.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyChaser.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
		case GREnemyChaser.Behavior.Patrol:
		case GREnemyChaser.Behavior.Investigate:
			this.ChooseNewBehavior();
			return;
		case GREnemyChaser.Behavior.Wander:
			this.abilityWander.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemyChaser.Behavior.Stagger:
		case GREnemyChaser.Behavior.Dying:
		case GREnemyChaser.Behavior.Attack:
		case GREnemyChaser.Behavior.Flashed:
			break;
		case GREnemyChaser.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			this.ChooseNewBehavior();
			break;
		case GREnemyChaser.Behavior.Search:
			this.ChooseNewBehavior();
			return;
		default:
			return;
		}
	}

	// Token: 0x06003188 RID: 12680 RVA: 0x0010D1B0 File Offset: 0x0010B3B0
	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
		{
			if (this.agent.targetPlayer != null)
			{
				Vector3 position = GRPlayer.Get(this.agent.targetPlayer).transform.position;
				Vector3 vector = position - base.transform.position;
				float magnitude = vector.magnitude;
				if (magnitude < this.attackRange)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Attack, false);
				}
				else if (this.canChaseJump && this.abilityJump.IsCoolDownOver(this.chaseJumpMinInterval) && magnitude > this.attackRange + this.minChaseJumpDistance && GRSenseLineOfSight.HasNavmeshLineOfSight(base.transform.position, position, 10f))
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
			this.TrySetBehavior(GREnemyChaser.Behavior.Chase);
			return;
		}
		this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
		if (this.investigateLocation != null)
		{
			this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
			this.SetBehavior(GREnemyChaser.Behavior.Investigate, false);
			return;
		}
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyChaser.Behavior.Patrol, false);
			return;
		}
		this.SetBehavior(GREnemyChaser.Behavior.Wander, false);
	}

	// Token: 0x06003189 RID: 12681 RVA: 0x0010D380 File Offset: 0x0010B580
	private void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x0600318A RID: 12682 RVA: 0x0010D3A0 File Offset: 0x0010B5A0
	private void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilityIdle.UpdateAuthority(dt);
			return;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.UpdateAuthority(dt);
			return;
		case GREnemyChaser.Behavior.Wander:
			this.abilityWander.UpdateAuthority(dt);
			return;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.UpdateAuthority(dt);
			if (this.abilityStagger.IsDone())
			{
				if (this.agent.targetPlayer == null)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Search, false);
					return;
				}
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Dying:
			this.abilityDie.UpdateAuthority(dt);
			return;
		case GREnemyChaser.Behavior.Chase:
		{
			this.abilityChase.UpdateAuthority(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyChaser.Behavior.Search, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float num = this.attackRange * this.attackRange;
				if ((grplayer.transform.position - base.transform.position).sqrMagnitude < num)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Attack, false);
					return;
				}
			}
			break;
		}
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.UpdateAuthority(dt);
			if (this.abilitySearch.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.UpdateAuthority(dt);
			if (this.abilityAttackSwipe.IsDone())
			{
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.UpdateAuthority(dt);
			if (this.abilityFlashed.IsDone())
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Search, false);
					return;
				}
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Investigate:
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
		case GREnemyChaser.Behavior.Jump:
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

	// Token: 0x0600318B RID: 12683 RVA: 0x0010D5C4 File Offset: 0x0010B7C4
	private void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilityIdle.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Wander:
			this.abilityWander.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Dying:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Chase:
			this.abilityChase.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600318C RID: 12684 RVA: 0x0010D69C File Offset: 0x0010B89C
	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState != GREnemyChaser.BodyState.Bones)
		{
			if (this.currBodyState == GREnemyChaser.BodyState.Shell && this.armor != null)
			{
				this.armor.PlayBlockFx(hit.hitEntityPosition);
			}
			return;
		}
		this.hp -= hit.hitAmount;
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
			this.SetBodyState(GREnemyChaser.BodyState.Destroyed, false);
			this.SetBehavior(GREnemyChaser.Behavior.Dying, false);
			return;
		}
		this.lastSeenTargetPosition = tool.transform.position;
		this.lastSeenTargetTime = Time.timeAsDouble;
		Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
		vector.y = 0f;
		this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
		this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
		this.TrySetBehavior(GREnemyChaser.Behavior.Stagger);
	}

	// Token: 0x0600318D RID: 12685 RVA: 0x0010D814 File Offset: 0x0010BA14
	public void InstantDeath()
	{
		this.hp = 0;
		this.SetBodyState(GREnemyChaser.BodyState.Destroyed, false);
		this.SetBehavior(GREnemyChaser.Behavior.Dying, false);
	}

	// Token: 0x0600318E RID: 12686 RVA: 0x0010D830 File Offset: 0x0010BA30
	public void OnHitByFlash(GRTool grTool, GameHitData hit)
	{
		if (this.currBodyState == GREnemyChaser.BodyState.Shell)
		{
			this.hp -= hit.hitAmount;
			if (this.armor != null)
			{
				this.armor.SetHp(this.hp);
			}
			if (this.hp <= 0)
			{
				if (this.armor != null)
				{
					this.armor.PlayDestroyFx(this.armor.transform.position);
				}
				this.SetBodyState(GREnemyChaser.BodyState.Bones, false);
				if (grTool.gameEntity.IsHeldByLocalPlayer())
				{
					PlayerGameEvents.MiscEvent("GRArmorBreak_" + base.name, 1);
				}
				if (grTool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage3))
				{
					this.armor.FragmentArmor();
				}
			}
			else if (grTool != null)
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.lastSeenTargetPosition = grTool.transform.position;
				this.lastSeenTargetTime = Time.timeAsDouble;
				Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
				vector.y = 0f;
				this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
				this.RefreshBody();
			}
			else
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.RefreshBody();
			}
		}
		GRToolFlash component = grTool.GetComponent<GRToolFlash>();
		if (component != null)
		{
			this.abilityFlashed.SetStunTime(component.stunDuration);
		}
		this.TrySetBehavior(GREnemyChaser.Behavior.Flashed);
	}

	// Token: 0x0600318F RID: 12687 RVA: 0x0010D9EA File Offset: 0x0010BBEA
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		Debug.Log(string.Format("Chaser On Hit By Shield dmg:{0} impulse:{1} size:{2}", hit.hitAmount, hit.hitImpulse, hit.hitImpulse.magnitude));
		this.OnHitByClub(tool, hit);
	}

	// Token: 0x06003190 RID: 12688 RVA: 0x0010DA2C File Offset: 0x0010BC2C
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyChaser.BodyState.Destroyed)
		{
			return;
		}
		if (this.currBehavior != GREnemyChaser.Behavior.Attack)
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

	// Token: 0x06003191 RID: 12689 RVA: 0x0010DB99 File Offset: 0x0010BD99
	private IEnumerator TryHitPlayer(GRPlayer player)
	{
		yield return new WaitForUpdate();
		if (this.currBehavior == GREnemyChaser.Behavior.Attack && player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position);
		}
		yield break;
	}

	// Token: 0x06003192 RID: 12690 RVA: 0x0010DBB0 File Offset: 0x0010BDB0
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		strings.Add(string.Format("speed: <color=\"yellow\">{0}<color=\"white\"> patrol node:<color=\"yellow\">{1}/{2}<color=\"white\">", this.navAgent.speed, this.abilityPatrol.nextPatrolNode, (this.abilityPatrol.GetPatrolPath() != null) ? this.abilityPatrol.GetPatrolPath().patrolNodes.Count : 0));
		strings.Add(string.Format("Dest: <color=\"yellow\">{0}<color=\"white\"> Pos: <color=\"yellow\">{1}<color=\"white\">", this.agent.navAgent.destination, this.agent.transform.position));
	}

	// Token: 0x06003193 RID: 12691 RVA: 0x0010DC90 File Offset: 0x0010BE90
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

	// Token: 0x06003194 RID: 12692 RVA: 0x0010DCFC File Offset: 0x0010BEFC
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyChaser.Behavior behavior = (GREnemyChaser.Behavior)reader.ReadByte();
		GREnemyChaser.BodyState bodyState = (GREnemyChaser.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		byte b = reader.ReadByte();
		int num2 = reader.ReadInt32();
		this.SetPatrolPath(this.entity.createData);
		this.SetNextPatrolNode((int)b);
		this.SetHP(num);
		this.SetBehavior(behavior, true);
		this.SetBodyState(bodyState, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(num2);
	}

	// Token: 0x06003195 RID: 12693 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06003196 RID: 12694 RVA: 0x0010DD70 File Offset: 0x0010BF70
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

	// Token: 0x04003F7D RID: 16253
	public GameEntity entity;

	// Token: 0x04003F7E RID: 16254
	public GameAgent agent;

	// Token: 0x04003F7F RID: 16255
	public GREnemy enemy;

	// Token: 0x04003F80 RID: 16256
	public GRArmorEnemy armor;

	// Token: 0x04003F81 RID: 16257
	public GameHittable hittable;

	// Token: 0x04003F82 RID: 16258
	[SerializeField]
	private GRAttributes attributes;

	// Token: 0x04003F83 RID: 16259
	public GRSenseNearby senseNearby;

	// Token: 0x04003F84 RID: 16260
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003F85 RID: 16261
	public Animation anim;

	// Token: 0x04003F86 RID: 16262
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003F87 RID: 16263
	public GRAbilityChase abilityChase;

	// Token: 0x04003F88 RID: 16264
	public GRAbilityIdle abilitySearch;

	// Token: 0x04003F89 RID: 16265
	public GRAbilityAttackSwipe abilityAttackSwipe;

	// Token: 0x04003F8A RID: 16266
	public GRAbilityStagger abilityStagger;

	// Token: 0x04003F8B RID: 16267
	public GRAbilityDie abilityDie;

	// Token: 0x04003F8C RID: 16268
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x04003F8D RID: 16269
	public GRAbilityPatrol abilityPatrol;

	// Token: 0x04003F8E RID: 16270
	public GRAbilityWander abilityWander;

	// Token: 0x04003F8F RID: 16271
	public GRAbilityFlashed abilityFlashed;

	// Token: 0x04003F90 RID: 16272
	public GRAbilityJump abilityJump;

	// Token: 0x04003F91 RID: 16273
	public List<Renderer> bones;

	// Token: 0x04003F92 RID: 16274
	public List<Renderer> always;

	// Token: 0x04003F93 RID: 16275
	public Transform coreMarker;

	// Token: 0x04003F94 RID: 16276
	public GRCollectible corePrefab;

	// Token: 0x04003F95 RID: 16277
	public Transform headTransform;

	// Token: 0x04003F96 RID: 16278
	public float turnSpeed = 540f;

	// Token: 0x04003F97 RID: 16279
	public SoundBankPlayer chaseSoundBank;

	// Token: 0x04003F98 RID: 16280
	public float attackRange = 1.5f;

	// Token: 0x04003F99 RID: 16281
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x04003F9A RID: 16282
	public NavMeshAgent navAgent;

	// Token: 0x04003F9B RID: 16283
	public AudioSource audioSource;

	// Token: 0x04003F9C RID: 16284
	public AudioClip damagedSound;

	// Token: 0x04003F9D RID: 16285
	public float damagedSoundVolume;

	// Token: 0x04003F9E RID: 16286
	public List<AudioClip> damagedSounds;

	// Token: 0x04003F9F RID: 16287
	private int damagedSoundIndex;

	// Token: 0x04003FA0 RID: 16288
	public GameObject fxDamaged;

	// Token: 0x04003FA1 RID: 16289
	private Vector3? investigateLocation;

	// Token: 0x04003FA2 RID: 16290
	private float lastStaggerTime;

	// Token: 0x04003FA3 RID: 16291
	public float staggerImmuneTime = 10f;

	// Token: 0x04003FA4 RID: 16292
	private Transform target;

	// Token: 0x04003FA5 RID: 16293
	[ReadOnly]
	public int hp;

	// Token: 0x04003FA6 RID: 16294
	[ReadOnly]
	public GREnemyChaser.Behavior currBehavior;

	// Token: 0x04003FA7 RID: 16295
	[ReadOnly]
	public GREnemyChaser.BodyState currBodyState;

	// Token: 0x04003FA8 RID: 16296
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04003FA9 RID: 16297
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x04003FAA RID: 16298
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x04003FAB RID: 16299
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x04003FAC RID: 16300
	public bool canChaseJump = true;

	// Token: 0x04003FAD RID: 16301
	public float chaseJumpDistance = 5f;

	// Token: 0x04003FAE RID: 16302
	public float chaseJumpMinInterval = 1f;

	// Token: 0x04003FAF RID: 16303
	public float minChaseJumpDistance = 2f;

	// Token: 0x04003FB0 RID: 16304
	public static RaycastHit[] visibilityHits = new RaycastHit[16];

	// Token: 0x04003FB1 RID: 16305
	private Rigidbody rigidBody;

	// Token: 0x04003FB2 RID: 16306
	private List<Collider> colliders;

	// Token: 0x04003FB3 RID: 16307
	private float lastHitPlayerTime;

	// Token: 0x04003FB4 RID: 16308
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x04003FB5 RID: 16309
	public float hearingRadius = 5f;

	// Token: 0x04003FB6 RID: 16310
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x04003FB7 RID: 16311
	private Coroutine tryHitPlayerCoroutine;

	// Token: 0x0200079A RID: 1946
	public enum Behavior
	{
		// Token: 0x04003FB9 RID: 16313
		Idle,
		// Token: 0x04003FBA RID: 16314
		Patrol,
		// Token: 0x04003FBB RID: 16315
		Wander,
		// Token: 0x04003FBC RID: 16316
		Stagger,
		// Token: 0x04003FBD RID: 16317
		Dying,
		// Token: 0x04003FBE RID: 16318
		Chase,
		// Token: 0x04003FBF RID: 16319
		Search,
		// Token: 0x04003FC0 RID: 16320
		Attack,
		// Token: 0x04003FC1 RID: 16321
		Flashed,
		// Token: 0x04003FC2 RID: 16322
		Investigate,
		// Token: 0x04003FC3 RID: 16323
		Jump,
		// Token: 0x04003FC4 RID: 16324
		Count
	}

	// Token: 0x0200079B RID: 1947
	public enum BodyState
	{
		// Token: 0x04003FC6 RID: 16326
		Destroyed,
		// Token: 0x04003FC7 RID: 16327
		Bones,
		// Token: 0x04003FC8 RID: 16328
		Shell,
		// Token: 0x04003FC9 RID: 16329
		Count
	}
}
