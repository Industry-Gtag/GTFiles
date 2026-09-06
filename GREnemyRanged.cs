using System;
using System.Collections.Generic;
using System.IO;
using CjLib;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

// Token: 0x020007A9 RID: 1961
public class GREnemyRanged : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameProjectileLauncher, IGameEntityDebugComponent
{
	// Token: 0x06003213 RID: 12819 RVA: 0x001117D0 File Offset: 0x0010F9D0
	private bool IsMoving()
	{
		return this.navAgent.velocity.sqrMagnitude > 0f;
	}

	// Token: 0x06003214 RID: 12820 RVA: 0x001117F8 File Offset: 0x0010F9F8
	private void SoftResetThrowableHead()
	{
		this.headRemoved = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
		this.headLightReset = true;
		this.spitterLightTurnOffTime = Time.timeAsDouble + this.spitterLightTurnOffDelay;
	}

	// Token: 0x06003215 RID: 12821 RVA: 0x00111864 File Offset: 0x0010FA64
	private void ForceResetThrowableHead()
	{
		this.headRemoved = false;
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x06003216 RID: 12822 RVA: 0x001118C8 File Offset: 0x0010FAC8
	private void ForceHeadToDeadState()
	{
		this.headRemoved = false;
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x06003217 RID: 12823 RVA: 0x0011192C File Offset: 0x0010FB2C
	private void EnableVFXForShoulderHead()
	{
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersLight.SetActive(true);
		this.spitterHeadOnShouldersVFX.SetActive(true);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x06003218 RID: 12824 RVA: 0x00111988 File Offset: 0x0010FB88
	private void EnableVFXForHeadInHand()
	{
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(false);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(true);
		this.spitterHeadInHandLight.SetActive(true);
		this.spitterHeadInHandVFX.SetActive(true);
	}

	// Token: 0x06003219 RID: 12825 RVA: 0x001119E4 File Offset: 0x0010FBE4
	private void DisableHeadInHand()
	{
		this.headLightReset = false;
		this.spitterHeadInHand.SetActive(false);
	}

	// Token: 0x0600321A RID: 12826 RVA: 0x001119FC File Offset: 0x0010FBFC
	private void DisableHeadOnShoulderAndHeadInHand()
	{
		this.headLightReset = false;
		this.headRemoved = false;
		this.spitterHeadOnShoulders.SetActive(false);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x0600321B RID: 12827 RVA: 0x00111A60 File Offset: 0x0010FC60
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		this.visibilityLayerMask = LayerMask.GetMask(new string[] { "Default" });
		this.senseNearby.Setup(this.headTransform, this.entity);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x0600321C RID: 12828 RVA: 0x00111B1C File Offset: 0x0010FD1C
	public void OnEntityInit()
	{
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityPatrol.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityFlashed.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityKeepDistance.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
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

	// Token: 0x0600321D RID: 12829 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x0600321E RID: 12830 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x0600321F RID: 12831 RVA: 0x00111D1C File Offset: 0x0010FF1C
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
		this.DestroyProjectile();
	}

	// Token: 0x06003220 RID: 12832 RVA: 0x00111D54 File Offset: 0x0010FF54
	public void Setup(long entityCreateData)
	{
		this.SetPatrolPath(entityCreateData);
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyRanged.Behavior.Patrol, true);
		}
		else
		{
			this.SetBehavior(GREnemyRanged.Behavior.Idle, true);
		}
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyRanged.BodyState.Shell, true);
		}
		else
		{
			this.SetBodyState(GREnemyRanged.BodyState.Bones, true);
		}
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
	}

	// Token: 0x06003221 RID: 12833 RVA: 0x00111DD2 File Offset: 0x0010FFD2
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyRanged.Behavior.Jump, false);
	}

	// Token: 0x06003222 RID: 12834 RVA: 0x00111DED File Offset: 0x0010FFED
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 11)
		{
			return;
		}
		this.SetBehavior((GREnemyRanged.Behavior)newState, false);
	}

	// Token: 0x06003223 RID: 12835 RVA: 0x00111E01 File Offset: 0x00110001
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyRanged.BodyState)newState, false);
	}

	// Token: 0x06003224 RID: 12836 RVA: 0x00111E14 File Offset: 0x00110014
	public void SetPatrolPath(long entityCreateData)
	{
		this.abilityPatrol.SetPatrolPath(GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(entityCreateData));
	}

	// Token: 0x06003225 RID: 12837 RVA: 0x00111E37 File Offset: 0x00110037
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x06003226 RID: 12838 RVA: 0x00111E40 File Offset: 0x00110040
	public bool TrySetBehavior(GREnemyRanged.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemyRanged.Behavior.Jump && newBehavior == GREnemyRanged.Behavior.Stagger)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x06003227 RID: 12839 RVA: 0x00111E5C File Offset: 0x0011005C
	public void SetBehavior(GREnemyRanged.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Patrol:
			this.abilityPatrol.Stop();
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.Stop();
			break;
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
			if (newBehavior != GREnemyRanged.Behavior.RangedAttack)
			{
				this.SoftResetThrowableHead();
			}
			break;
		case GREnemyRanged.Behavior.RangedAttack:
			if (newBehavior != GREnemyRanged.Behavior.RangedAttackCooldown)
			{
				this.ForceResetThrowableHead();
			}
			break;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			this.ForceResetThrowableHead();
			this.abilityKeepDistance.Stop();
			break;
		case GREnemyRanged.Behavior.Flashed:
			this.abilityFlashed.Stop();
			break;
		case GREnemyRanged.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyRanged.Behavior.Jump:
			this.abilityJump.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Idle:
			this.targetPlayer = null;
			this.PlayAnim("GREnemyRangedIdleSearch", 0.1f, 1f);
			break;
		case GREnemyRanged.Behavior.Patrol:
			this.targetPlayer = null;
			this.abilityPatrol.Start();
			break;
		case GREnemyRanged.Behavior.Search:
			this.targetPlayer = null;
			this.PlayAnim("GREnemyRangedWalk", 0.1f, 1f);
			this.navAgent.speed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
			this.lastMoving = false;
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.Start();
			break;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.Start();
			if (this.entity.IsAuthority())
			{
				this.entity.manager.RequestCreateItem(this.corePrefab.gameObject.name.GetStaticHash(), this.coreMarker.position, this.coreMarker.rotation, 0L);
			}
			break;
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
			this.PlayAnim("GREnemyRangedWalk", 0.1f, 1f);
			this.navAgent.speed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.ChaseSpeed);
			this.EnableVFXForShoulderHead();
			this.chaseAbilitySound.Play(this.audioSecondarySource);
			break;
		case GREnemyRanged.Behavior.RangedAttack:
			this.PlayAnim("GREnemyRangedAttack01", 0.1f, 1f);
			this.navAgent.speed = 0f;
			this.navAgent.velocity = Vector3.zero;
			this.headRemovaltime = PhotonNetwork.Time + (double)this.headRemovalFrame;
			this.attackAbilitySound.Play(this.audioSource);
			break;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			this.lastMoving = true;
			this.abilityKeepDistance.SetTargetPlayer(this.targetPlayer);
			this.abilityKeepDistance.Start();
			break;
		case GREnemyRanged.Behavior.Flashed:
			this.abilityFlashed.Start();
			break;
		case GREnemyRanged.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyRanged.Behavior.Jump:
			this.abilityJump.Start();
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x06003228 RID: 12840 RVA: 0x00112164 File Offset: 0x00110364
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null)
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x06003229 RID: 12841 RVA: 0x00112194 File Offset: 0x00110394
	public void SetBodyState(GREnemyRanged.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyRanged.BodyState.Destroyed:
		{
			this.ForceResetThrowableHead();
			for (int i = 0; i < this.colliders.Count; i++)
			{
				this.colliders[i].enabled = true;
			}
			break;
		}
		case GREnemyRanged.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyRanged.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyRanged.BodyState.Destroyed:
			this.DisableHeadOnShoulderAndHeadInHand();
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyRanged.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyRanged.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x0600322A RID: 12842 RVA: 0x001122A4 File Offset: 0x001104A4
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyRanged.BodyState.Destroyed:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, true);
			this.DisableHeadOnShoulderAndHeadInHand();
			return;
		case GREnemyRanged.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyRanged.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600322B RID: 12843 RVA: 0x00112344 File Offset: 0x00110544
	private void Update()
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(Time.deltaTime);
		}
		else
		{
			this.OnUpdateRemote(Time.deltaTime);
		}
		this.UpdateShared();
	}

	// Token: 0x0600322C RID: 12844 RVA: 0x00112374 File Offset: 0x00110574
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		if (!GhostReactorManager.AggroDisabled)
		{
			GREnemyRanged.Behavior behavior = this.currBehavior;
			if (behavior > GREnemyRanged.Behavior.Search)
			{
				if (behavior == GREnemyRanged.Behavior.RangedAttackCooldown)
				{
					this.abilityKeepDistance.Think(dt);
					this.UpdateTarget();
					return;
				}
				if (behavior != GREnemyRanged.Behavior.Investigate)
				{
					return;
				}
			}
			this.UpdateTarget();
		}
	}

	// Token: 0x0600322D RID: 12845 RVA: 0x001123C4 File Offset: 0x001105C4
	private void UpdateTarget()
	{
		this.bestTargetPlayer = null;
		this.bestTargetNetPlayer = null;
		GREnemyRanged.tempRigs.Clear();
		GREnemyRanged.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyRanged.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyRanged.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		if (vrrig != null)
		{
			GRPlayer component = vrrig.GetComponent<GRPlayer>();
			if (component != null && component.State != GRPlayer.GRPlayerState.Ghost)
			{
				this.bestTargetPlayer = component;
				this.bestTargetNetPlayer = vrrig.OwningNetPlayer;
				this.lastSeenTargetTime = Time.timeAsDouble;
				this.lastSeenTargetPosition = vrrig.transform.position;
			}
		}
	}

	// Token: 0x0600322E RID: 12846 RVA: 0x00112478 File Offset: 0x00110678
	private void ChooseNewBehavior()
	{
		if (this.bestTargetPlayer != null && Time.timeAsDouble - this.lastSeenTargetTime < (double)this.sightLostFollowStopTime)
		{
			this.targetPlayer = this.bestTargetNetPlayer;
			this.lastSeenTargetTime = Time.timeAsDouble;
			this.investigateLocation = null;
			this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
			return;
		}
		if (Time.timeAsDouble - this.lastSeenTargetTime < (double)this.searchTime)
		{
			this.SetBehavior(GREnemyRanged.Behavior.Search, false);
			return;
		}
		this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
		if (this.investigateLocation != null)
		{
			this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
			this.SetBehavior(GREnemyRanged.Behavior.Investigate, false);
			return;
		}
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyRanged.Behavior.Patrol, false);
			return;
		}
		this.SetBehavior(GREnemyRanged.Behavior.Idle, false);
	}

	// Token: 0x0600322F RID: 12847 RVA: 0x00112560 File Offset: 0x00110760
	private void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Idle:
			this.ChooseNewBehavior();
			break;
		case GREnemyRanged.Behavior.Patrol:
			this.abilityPatrol.UpdateAuthority(dt);
			this.ChooseNewBehavior();
			break;
		case GREnemyRanged.Behavior.Search:
			this.UpdateSearch();
			this.ChooseNewBehavior();
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.UpdateAuthority(dt);
			if (this.abilityStagger.IsDone())
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyRanged.Behavior.Search, false);
				}
				else
				{
					this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
				}
			}
			break;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.UpdateAuthority(dt);
			break;
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
			if (this.targetPlayer != null)
			{
				GRPlayer grplayer = GRPlayer.Get(this.targetPlayer.ActorNumber);
				if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
				{
					Vector3 position = grplayer.transform.position;
					Vector3 position2 = base.transform.position;
					float magnitude = (position - position2).magnitude;
					if (magnitude > this.loseSightDist)
					{
						this.ChooseNewBehavior();
					}
					else
					{
						float num = Vector3.Distance(position, this.headTransform.position);
						bool flag = false;
						if (num < this.sightDist)
						{
							flag = Physics.RaycastNonAlloc(new Ray(this.headTransform.position, position - this.headTransform.position), GREnemyChaser.visibilityHits, num, this.visibilityLayerMask.value, QueryTriggerInteraction.Ignore) < 1;
						}
						if (flag)
						{
							this.lastSeenTargetPosition = position;
							this.lastSeenTargetTime = Time.timeAsDouble;
						}
						if (Time.timeAsDouble - this.lastSeenTargetTime < (double)this.sightLostFollowStopTime)
						{
							this.searchPosition = position;
							this.agent.RequestDestination(this.lastSeenTargetPosition);
							if (flag)
							{
								this.rangedTargetPosition = position;
								Vector3 vector = Vector3.up * 0.4f;
								this.rangedTargetPosition += vector;
								if (magnitude < this.rangedAttackDistMax)
								{
									this.behaviorEndTime = Time.timeAsDouble + (double)this.rangedAttackChargeTime;
									this.SetBehavior(GREnemyRanged.Behavior.RangedAttack, false);
									GhostReactorManager.Get(this.entity).RequestFireProjectile(this.entity.id, this.rangedProjectileFirePoint.position, this.rangedTargetPosition, PhotonNetwork.Time + (double)this.rangedAttackChargeTime);
								}
							}
						}
						else
						{
							this.ChooseNewBehavior();
						}
					}
				}
			}
			break;
		case GREnemyRanged.Behavior.RangedAttack:
			if (Time.timeAsDouble > this.behaviorEndTime)
			{
				if (this.targetPlayer != null)
				{
					GRPlayer grplayer2 = GRPlayer.Get(this.targetPlayer.ActorNumber);
					if (grplayer2 != null && grplayer2.State == GRPlayer.GRPlayerState.Alive)
					{
						this.rangedTargetPosition = grplayer2.transform.position;
					}
				}
				this.SetBehavior(GREnemyRanged.Behavior.RangedAttackCooldown, false);
				this.behaviorEndTime = Time.timeAsDouble + (double)this.rangedAttackRecoverTime;
			}
			break;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			if (Time.timeAsDouble > this.behaviorEndTime)
			{
				this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
				this.behaviorEndTime = Time.timeAsDouble;
			}
			else
			{
				this.abilityKeepDistance.UpdateAuthority(dt);
			}
			break;
		case GREnemyRanged.Behavior.Flashed:
			this.abilityFlashed.UpdateAuthority(dt);
			if (this.abilityFlashed.IsDone())
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyRanged.Behavior.Search, false);
				}
				else
				{
					this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
				}
			}
			break;
		case GREnemyRanged.Behavior.Investigate:
			this.abilityInvestigate.UpdateAuthority(dt);
			if (GhostReactorManager.noiseDebugEnabled)
			{
				DebugUtil.DrawLine(base.transform.position, this.abilityInvestigate.GetTargetPos(), Color.green, true);
			}
			this.ChooseNewBehavior();
			break;
		case GREnemyRanged.Behavior.Jump:
			this.abilityJump.UpdateAuthority(dt);
			if (this.abilityJump.IsDone())
			{
				this.ChooseNewBehavior();
			}
			break;
		}
		GameAgent.UpdateFacing(base.transform, this.navAgent, this.targetPlayer, this.turnSpeed);
	}

	// Token: 0x06003230 RID: 12848 RVA: 0x0011293C File Offset: 0x00110B3C
	private void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Patrol:
			this.abilityPatrol.UpdateRemote(dt);
			return;
		case GREnemyRanged.Behavior.Search:
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
		case GREnemyRanged.Behavior.RangedAttack:
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			this.abilityKeepDistance.UpdateRemote(dt);
			return;
		case GREnemyRanged.Behavior.Flashed:
			this.abilityFlashed.UpdateRemote(dt);
			return;
		case GREnemyRanged.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			if (GhostReactorManager.noiseDebugEnabled)
			{
				DebugUtil.DrawLine(base.transform.position, this.abilityInvestigate.GetTargetPos(), Color.green, true);
				return;
			}
			break;
		case GREnemyRanged.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			break;
		default:
			return;
		}
	}

	// Token: 0x06003231 RID: 12849 RVA: 0x00112A04 File Offset: 0x00110C04
	public void UpdateShared()
	{
		if (this.rangedAttackQueued)
		{
			if (!this.headRemoved && this.currBehavior == GREnemyRanged.Behavior.RangedAttack && PhotonNetwork.Time >= this.headRemovaltime)
			{
				this.headRemoved = true;
				this.EnableVFXForHeadInHand();
			}
			if (PhotonNetwork.Time > this.queuedFiringTime)
			{
				this.rangedAttackQueued = false;
				this.FireRangedAttack(this.queuedFiringPosition, this.queuedTargetPosition);
			}
		}
		if (this.headLightReset && Time.timeAsDouble > this.spitterLightTurnOffTime)
		{
			this.spitterHeadOnShouldersLight.SetActive(false);
			this.headLightReset = false;
		}
	}

	// Token: 0x06003232 RID: 12850 RVA: 0x00112A94 File Offset: 0x00110C94
	private void UpdateSearch()
	{
		Vector3 vector = this.searchPosition - base.transform.position;
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
		if (vector2.sqrMagnitude < 0.15f)
		{
			Vector3 vector3 = this.lastSeenTargetPosition - this.searchPosition;
			vector3.y = 0f;
			this.searchPosition = this.lastSeenTargetPosition + vector3;
		}
		if (this.IsMoving())
		{
			if (!this.lastMoving)
			{
				this.PlayAnim("GREnemyRangedWalk", 0.1f, 1f);
				this.lastMoving = true;
			}
		}
		else if (this.lastMoving)
		{
			this.PlayAnim("GREnemyRangedWalk", 0.1f, 1f);
			this.lastMoving = false;
		}
		this.agent.RequestDestination(this.searchPosition);
		if (Time.timeAsDouble - this.lastSeenTargetTime > (double)this.searchTime)
		{
			this.ChooseNewBehavior();
		}
	}

	// Token: 0x06003233 RID: 12851 RVA: 0x00112B8C File Offset: 0x00110D8C
	private void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState != GREnemyRanged.BodyState.Bones)
		{
			if (this.currBodyState == GREnemyRanged.BodyState.Shell && this.armor != null)
			{
				this.armor.PlayBlockFx(hit.hitEntityPosition);
			}
			return;
		}
		this.hp -= hit.hitAmount;
		this.audioSource.PlayOneShot(this.damagedSound, this.damagedSoundVolume);
		if (this.fxDamaged != null)
		{
			this.fxDamaged.SetActive(false);
			this.fxDamaged.SetActive(true);
		}
		if (this.hp <= 0)
		{
			this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hit.hitByEntityId));
			this.SetBodyState(GREnemyRanged.BodyState.Destroyed, false);
			this.SetBehavior(GREnemyRanged.Behavior.Dying, false);
			return;
		}
		this.lastSeenTargetPosition = tool.transform.position;
		this.lastSeenTargetTime = Time.timeAsDouble;
		Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
		vector.y = 0f;
		this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
		this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
		this.TrySetBehavior(GREnemyRanged.Behavior.Stagger);
	}

	// Token: 0x06003234 RID: 12852 RVA: 0x00112CCE File Offset: 0x00110ECE
	public void InstantDeath()
	{
		this.hp = 0;
		this.SetBodyState(GREnemyRanged.BodyState.Destroyed, false);
		this.SetBehavior(GREnemyRanged.Behavior.Dying, false);
	}

	// Token: 0x06003235 RID: 12853 RVA: 0x00112CE8 File Offset: 0x00110EE8
	private void OnHitByFlash(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState == GREnemyRanged.BodyState.Shell)
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
				this.SetBodyState(GREnemyRanged.BodyState.Bones, false);
				if (tool.gameEntity.IsHeldByLocalPlayer())
				{
					PlayerGameEvents.MiscEvent("GRArmorBreak_" + base.name, 1);
				}
				if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage3))
				{
					this.armor.FragmentArmor();
				}
			}
			else if (tool != null)
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.lastSeenTargetPosition = tool.transform.position;
				this.lastSeenTargetTime = Time.timeAsDouble;
				Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
				vector.y = 0f;
				this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
				this.SetBehavior(GREnemyRanged.Behavior.Search, false);
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
		GRToolFlash component = tool.GetComponent<GRToolFlash>();
		if (component != null)
		{
			this.abilityFlashed.SetStunTime(component.stunDuration);
		}
		this.SetBehavior(GREnemyRanged.Behavior.Flashed, false);
	}

	// Token: 0x06003236 RID: 12854 RVA: 0x00112EAA File Offset: 0x001110AA
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	// Token: 0x06003237 RID: 12855 RVA: 0x00112EB4 File Offset: 0x001110B4
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

	// Token: 0x06003238 RID: 12856 RVA: 0x00112F20 File Offset: 0x00111120
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyRanged.Behavior behavior = (GREnemyRanged.Behavior)reader.ReadByte();
		GREnemyRanged.BodyState bodyState = (GREnemyRanged.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		byte b = reader.ReadByte();
		int num2 = reader.ReadInt32();
		this.SetPatrolPath((long)((int)this.entity.createData));
		this.abilityPatrol.SetNextPatrolNode((int)b);
		this.SetHP(num);
		this.SetBehavior(behavior, true);
		this.SetBodyState(bodyState, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(num2);
	}

	// Token: 0x06003239 RID: 12857 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x0600323A RID: 12858 RVA: 0x00112F9C File Offset: 0x0011119C
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

	// Token: 0x0600323B RID: 12859 RVA: 0x00112FFD File Offset: 0x001111FD
	public void RequestRangedAttack(Vector3 firingPosition, Vector3 targetPosition, double fireTime)
	{
		this.rangedAttackQueued = true;
		this.queuedFiringTime = fireTime;
		this.queuedFiringPosition = firingPosition;
		this.queuedTargetPosition = targetPosition;
	}

	// Token: 0x0600323C RID: 12860 RVA: 0x0011301C File Offset: 0x0011121C
	private void DestroyProjectile()
	{
		if (this.entity.IsAuthority() && this.rangedProjectileInstance != null)
		{
			GameEntity component = this.rangedProjectileInstance.GetComponent<GameEntity>();
			if (component != null)
			{
				component.manager.RequestDestroyItem(component.id);
			}
		}
	}

	// Token: 0x0600323D RID: 12861 RVA: 0x0011306C File Offset: 0x0011126C
	private void FireRangedAttack(Vector3 launchPosition, Vector3 targetPosition)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		this.DisableHeadInHand();
		this.DestroyProjectile();
		Vector3 vector;
		if (GREnemyRanged.CalculateLaunchDirection(launchPosition, targetPosition, this.projectileSpeed, out vector))
		{
			this.entity.manager.RequestCreateItem(this.rangedProjectilePrefab.name.GetStaticHash(), launchPosition, Quaternion.LookRotation(vector, Vector3.up), (long)this.entity.GetNetId());
		}
	}

	// Token: 0x0600323E RID: 12862 RVA: 0x001130E0 File Offset: 0x001112E0
	public static bool CalculateLaunchDirection(Vector3 startPos, Vector3 targetPos, float speed, out Vector3 direction)
	{
		direction = Vector3.zero;
		Vector3 vector = targetPos - startPos;
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
		float magnitude = vector2.magnitude;
		Vector3 normalized = vector2.normalized;
		float y = vector.y;
		float num = 9.8f;
		float num2 = speed * speed;
		float num3 = num2 * num2 - num * (num * magnitude * magnitude + 2f * y * num2);
		if (num3 < 0f)
		{
			return false;
		}
		int num4 = 0;
		float num5 = Mathf.Sqrt(num3);
		float num6 = (num2 + num5) / (num * magnitude);
		float num7 = (num2 - num5) / (num * magnitude);
		float num8 = num2 / (num6 * num6 + 1f);
		float num9 = num2 / (num7 * num7 + 1f);
		float num10 = ((num4 != 0) ? Mathf.Min(num8, num9) : Mathf.Max(num8, num9));
		float num11 = ((num4 != 0) ? ((num8 < num9) ? Mathf.Sign(num6) : Mathf.Sign(num7)) : ((num8 > num9) ? Mathf.Sign(num6) : Mathf.Sign(num7)));
		float num12 = Mathf.Sqrt(num10);
		float num13 = Mathf.Sqrt(Mathf.Abs(num2 - num10));
		direction = (normalized * num12 + new Vector3(0f, num13 * num11, 0f)).normalized;
		return true;
	}

	// Token: 0x0600323F RID: 12863 RVA: 0x0011323C File Offset: 0x0011143C
	public void OnProjectileInit(GRRangedEnemyProjectile projectile)
	{
		this.rangedProjectileInstance = projectile.gameObject;
	}

	// Token: 0x06003240 RID: 12864 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnProjectileHit(GRRangedEnemyProjectile projectile, Collision collision)
	{
	}

	// Token: 0x06003241 RID: 12865 RVA: 0x0011324C File Offset: 0x0011144C
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		strings.Add(string.Format("speed: <color=\"yellow\">{0}<color=\"white\"> patrol node:<color=\"yellow\">{1}/{2}<color=\"white\">", this.navAgent.speed, this.abilityPatrol.nextPatrolNode, (this.abilityPatrol.GetPatrolPath() != null) ? this.abilityPatrol.GetPatrolPath().patrolNodes.Count : 0));
		if (this.targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(this.targetPlayer.ActorNumber);
			if (grplayer != null)
			{
				float magnitude = (grplayer.transform.position - base.transform.position).magnitude;
				strings.Add(string.Format("TargetDis: <color=\"yellow\">{0}<color=\"white\"> ", magnitude));
			}
		}
	}

	// Token: 0x040040A2 RID: 16546
	public GameEntity entity;

	// Token: 0x040040A3 RID: 16547
	public GameAgent agent;

	// Token: 0x040040A4 RID: 16548
	public GREnemy enemy;

	// Token: 0x040040A5 RID: 16549
	public GRArmorEnemy armor;

	// Token: 0x040040A6 RID: 16550
	public GameHittable hittable;

	// Token: 0x040040A7 RID: 16551
	public GRAttributes attributes;

	// Token: 0x040040A8 RID: 16552
	public Animation anim;

	// Token: 0x040040A9 RID: 16553
	public GRSenseNearby senseNearby;

	// Token: 0x040040AA RID: 16554
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x040040AB RID: 16555
	public GRAbilityStagger abilityStagger;

	// Token: 0x040040AC RID: 16556
	public GRAbilityDie abilityDie;

	// Token: 0x040040AD RID: 16557
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x040040AE RID: 16558
	public GRAbilityPatrol abilityPatrol;

	// Token: 0x040040AF RID: 16559
	public GRAbilityFlashed abilityFlashed;

	// Token: 0x040040B0 RID: 16560
	public GRAbilityKeepDistance abilityKeepDistance;

	// Token: 0x040040B1 RID: 16561
	public GRAbilityJump abilityJump;

	// Token: 0x040040B2 RID: 16562
	public List<Renderer> bones;

	// Token: 0x040040B3 RID: 16563
	public List<Renderer> always;

	// Token: 0x040040B4 RID: 16564
	public Transform coreMarker;

	// Token: 0x040040B5 RID: 16565
	public GRCollectible corePrefab;

	// Token: 0x040040B6 RID: 16566
	public Transform headTransform;

	// Token: 0x040040B7 RID: 16567
	public float sightDist;

	// Token: 0x040040B8 RID: 16568
	public float loseSightDist;

	// Token: 0x040040B9 RID: 16569
	public float sightFOV;

	// Token: 0x040040BA RID: 16570
	public float sightLostFollowStopTime = 0.5f;

	// Token: 0x040040BB RID: 16571
	public float searchTime = 5f;

	// Token: 0x040040BC RID: 16572
	public float hearingRadius = 5f;

	// Token: 0x040040BD RID: 16573
	public float turnSpeed = 540f;

	// Token: 0x040040BE RID: 16574
	public Color chaseColor = Color.red;

	// Token: 0x040040BF RID: 16575
	public AbilitySound attackAbilitySound;

	// Token: 0x040040C0 RID: 16576
	public AbilitySound chaseAbilitySound;

	// Token: 0x040040C1 RID: 16577
	public float rangedAttackDistMin = 6f;

	// Token: 0x040040C2 RID: 16578
	public float rangedAttackDistMax = 8f;

	// Token: 0x040040C3 RID: 16579
	public float rangedAttackChargeTime = 0.5f;

	// Token: 0x040040C4 RID: 16580
	public float rangedAttackRecoverTime = 2f;

	// Token: 0x040040C5 RID: 16581
	public float projectileSpeed = 5f;

	// Token: 0x040040C6 RID: 16582
	public float projectileHitRadius = 1f;

	// Token: 0x040040C7 RID: 16583
	public GameObject rangedProjectilePrefab;

	// Token: 0x040040C8 RID: 16584
	public Transform rangedProjectileFirePoint;

	// Token: 0x040040C9 RID: 16585
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x040040CA RID: 16586
	public NavMeshAgent navAgent;

	// Token: 0x040040CB RID: 16587
	public AudioSource audioSource;

	// Token: 0x040040CC RID: 16588
	public AudioSource audioSecondarySource;

	// Token: 0x040040CD RID: 16589
	public AudioClip damagedSound;

	// Token: 0x040040CE RID: 16590
	public float damagedSoundVolume;

	// Token: 0x040040CF RID: 16591
	public GameObject fxDamaged;

	// Token: 0x040040D0 RID: 16592
	public bool lastMoving;

	// Token: 0x040040D1 RID: 16593
	private Vector3? investigateLocation;

	// Token: 0x040040D2 RID: 16594
	public bool debugLog;

	// Token: 0x040040D3 RID: 16595
	public GameObject spitterHeadOnShoulders;

	// Token: 0x040040D4 RID: 16596
	public GameObject spitterHeadOnShouldersLight;

	// Token: 0x040040D5 RID: 16597
	public GameObject spitterHeadOnShouldersVFX;

	// Token: 0x040040D6 RID: 16598
	public GameObject spitterHeadInHand;

	// Token: 0x040040D7 RID: 16599
	public GameObject spitterHeadInHandLight;

	// Token: 0x040040D8 RID: 16600
	public GameObject spitterHeadInHandVFX;

	// Token: 0x040040D9 RID: 16601
	public double spitterLightTurnOffDelay = 0.75;

	// Token: 0x040040DA RID: 16602
	private bool headLightReset;

	// Token: 0x040040DB RID: 16603
	private double spitterLightTurnOffTime;

	// Token: 0x040040DC RID: 16604
	[FormerlySerializedAs("headRemovalInterval")]
	public float headRemovalFrame = 0.23333333f;

	// Token: 0x040040DD RID: 16605
	private double headRemovaltime;

	// Token: 0x040040DE RID: 16606
	private bool headRemoved;

	// Token: 0x040040DF RID: 16607
	private Transform target;

	// Token: 0x040040E0 RID: 16608
	[ReadOnly]
	public int hp;

	// Token: 0x040040E1 RID: 16609
	[ReadOnly]
	public GREnemyRanged.Behavior currBehavior;

	// Token: 0x040040E2 RID: 16610
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x040040E3 RID: 16611
	[ReadOnly]
	public GREnemyRanged.BodyState currBodyState;

	// Token: 0x040040E4 RID: 16612
	[ReadOnly]
	public int nextPatrolNode;

	// Token: 0x040040E5 RID: 16613
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x040040E6 RID: 16614
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x040040E7 RID: 16615
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x040040E8 RID: 16616
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x040040E9 RID: 16617
	[ReadOnly]
	public Vector3 rangedFiringPosition;

	// Token: 0x040040EA RID: 16618
	[ReadOnly]
	public Vector3 rangedTargetPosition;

	// Token: 0x040040EB RID: 16619
	[ReadOnly]
	private GRPlayer bestTargetPlayer;

	// Token: 0x040040EC RID: 16620
	[ReadOnly]
	private NetPlayer bestTargetNetPlayer;

	// Token: 0x040040ED RID: 16621
	private bool rangedAttackQueued;

	// Token: 0x040040EE RID: 16622
	private double queuedFiringTime;

	// Token: 0x040040EF RID: 16623
	private Vector3 queuedFiringPosition;

	// Token: 0x040040F0 RID: 16624
	private Vector3 queuedTargetPosition;

	// Token: 0x040040F1 RID: 16625
	private GameObject rangedProjectileInstance;

	// Token: 0x040040F2 RID: 16626
	private bool projectileHasImpacted;

	// Token: 0x040040F3 RID: 16627
	private double projectileImpactTime;

	// Token: 0x040040F4 RID: 16628
	private Rigidbody rigidBody;

	// Token: 0x040040F5 RID: 16629
	private List<Collider> colliders;

	// Token: 0x040040F6 RID: 16630
	private LayerMask visibilityLayerMask;

	// Token: 0x040040F7 RID: 16631
	private Color defaultColor;

	// Token: 0x040040F8 RID: 16632
	private float lastHitPlayerTime;

	// Token: 0x040040F9 RID: 16633
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x040040FA RID: 16634
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x020007AA RID: 1962
	public enum Behavior
	{
		// Token: 0x040040FC RID: 16636
		Idle,
		// Token: 0x040040FD RID: 16637
		Patrol,
		// Token: 0x040040FE RID: 16638
		Search,
		// Token: 0x040040FF RID: 16639
		Stagger,
		// Token: 0x04004100 RID: 16640
		Dying,
		// Token: 0x04004101 RID: 16641
		SeekRangedAttackPosition,
		// Token: 0x04004102 RID: 16642
		RangedAttack,
		// Token: 0x04004103 RID: 16643
		RangedAttackCooldown,
		// Token: 0x04004104 RID: 16644
		Flashed,
		// Token: 0x04004105 RID: 16645
		Investigate,
		// Token: 0x04004106 RID: 16646
		Jump,
		// Token: 0x04004107 RID: 16647
		Count
	}

	// Token: 0x020007AB RID: 1963
	public enum BodyState
	{
		// Token: 0x04004109 RID: 16649
		Destroyed,
		// Token: 0x0400410A RID: 16650
		Bones,
		// Token: 0x0400410B RID: 16651
		Shell,
		// Token: 0x0400410C RID: 16652
		Count
	}
}
