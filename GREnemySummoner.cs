using System;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020007AC RID: 1964
public class GREnemySummoner : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameEntityDebugComponent, IGameAgentComponent, IGRSummoningEntity
{
	// Token: 0x06003244 RID: 12868 RVA: 0x0011340C File Offset: 0x0011160C
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		this.trackedEntities = new List<int>();
		base.GetComponentsInChildren<Collider>(this.colliders);
		this.agent = base.GetComponent<GameAgent>();
		this.entity = base.GetComponent<GameEntity>();
		this.enemy = base.GetComponent<GREnemy>();
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.behaviorStartTime = -1.0;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.senseNearby.Setup(this.headTransform, this.entity);
	}

	// Token: 0x06003245 RID: 12869 RVA: 0x001134D0 File Offset: 0x001116D0
	public void OnEntityInit()
	{
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource, null, null, null);
		this.abilityWander.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilitySummon.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityKeepDistance.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityMoveToTarget.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityFlashed.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.SetBehavior(GREnemySummoner.Behavior.Idle, true);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry grbonusEntry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(grbonusEntry);
			}
		}
		this.SetHP(this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax));
		this.navAgent.speed = (float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.PatrolSpeed);
		this.agent.navAgent.autoTraverseOffMeshLink = false;
		this.agent.onJumpRequested += this.OnAgentJumpRequested;
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemySummoner.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemySummoner.BodyState.Bones, true);
	}

	// Token: 0x06003246 RID: 12870 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06003247 RID: 12871 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06003248 RID: 12872 RVA: 0x00113784 File Offset: 0x00111984
	private void OnDestroy()
	{
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x06003249 RID: 12873 RVA: 0x0011379D File Offset: 0x0011199D
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemySummoner.Behavior.Jump, false);
	}

	// Token: 0x0600324A RID: 12874 RVA: 0x001137B7 File Offset: 0x001119B7
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 10)
		{
			return;
		}
		this.SetBehavior((GREnemySummoner.Behavior)newState, false);
	}

	// Token: 0x0600324B RID: 12875 RVA: 0x001137CB File Offset: 0x001119CB
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x0600324C RID: 12876 RVA: 0x001137D4 File Offset: 0x001119D4
	public bool TrySetBehavior(GREnemySummoner.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemySummoner.Behavior.Jump && newBehavior == GREnemySummoner.Behavior.Stagger)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x0600324D RID: 12877 RVA: 0x001137F0 File Offset: 0x001119F0
	public void SetBehavior(GREnemySummoner.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.Stop();
			break;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemySummoner.Behavior.Destroyed:
			this.abilityDie.Stop();
			break;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.Stop();
			if (this.summonLight != null)
			{
				this.summonLight.gameObject.SetActive(false);
			}
			break;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.Stop();
			break;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.Stop();
			break;
		case GREnemySummoner.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemySummoner.Behavior.Jump:
			this.abilityJump.Stop();
			break;
		case GREnemySummoner.Behavior.Flashed:
			this.abilityFlashed.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		this.behaviorStartTime = Time.timeAsDouble;
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			this.abilityIdle.Start();
			break;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.Start();
			this.soundWander.Play(this.audioSource);
			break;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.Start();
			break;
		case GREnemySummoner.Behavior.Destroyed:
			if (this.entity.IsAuthority())
			{
				this.entity.manager.RequestCreateItem(this.corePrefab.gameObject.name.GetStaticHash(), this.coreMarker.position, this.coreMarker.rotation, 0L);
			}
			this.abilityDie.Start();
			break;
		case GREnemySummoner.Behavior.Summon:
			if (this.summonLight != null)
			{
				this.summonLight.gameObject.SetActive(true);
			}
			this.lastSummonTime = Time.timeAsDouble;
			this.abilitySummon.SetLookAtTarget(this.GetPlayerTransform(this.agent.targetPlayer));
			this.abilitySummon.Start();
			break;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.SetTargetPlayer(this.agent.targetPlayer);
			this.abilityKeepDistance.Start();
			break;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.SetTarget(this.GetPlayerTransform(this.agent.targetPlayer));
			this.abilityMoveToTarget.Start();
			break;
		case GREnemySummoner.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemySummoner.Behavior.Jump:
			this.abilityJump.Start();
			break;
		case GREnemySummoner.Behavior.Flashed:
			this.abilityFlashed.Start();
			break;
		}
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x0600324E RID: 12878 RVA: 0x00113AB4 File Offset: 0x00111CB4
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x0600324F RID: 12879 RVA: 0x00113AC4 File Offset: 0x00111CC4
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		this.lastUpdateTime = Time.time;
		GREnemySummoner.tempRigs.Clear();
		GREnemySummoner.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemySummoner.tempRigs);
		this.senseNearby.UpdateNearby(GREnemySummoner.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			this.abilityIdle.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemySummoner.Behavior.Stagger:
		case GREnemySummoner.Behavior.Destroyed:
			break;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.Think(dt);
			if (this.abilitySummon.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.Think(dt);
			this.ChooseNewBehavior();
			break;
		case GREnemySummoner.Behavior.Investigate:
			this.abilityInvestigate.Think(dt);
			this.ChooseNewBehavior();
			return;
		default:
			return;
		}
	}

	// Token: 0x06003250 RID: 12880 RVA: 0x00113C00 File Offset: 0x00111E00
	public bool CanSummon()
	{
		return !GhostReactorManager.AggroDisabled && (this.currBehavior != GREnemySummoner.Behavior.Summon || !this.abilitySummon.IsDone()) && Time.timeAsDouble - this.lastSummonTime >= (double)this.minSummonInterval && this.trackedEntities.Count < this.maxSimultaneousSummonedEntities;
	}

	// Token: 0x06003251 RID: 12881 RVA: 0x00113C58 File Offset: 0x00111E58
	public Transform GetPlayerTransform(NetPlayer targetPlayer)
	{
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				return grplayer.transform;
			}
		}
		return null;
	}

	// Token: 0x06003252 RID: 12882 RVA: 0x00113C90 File Offset: 0x00111E90
	private void ChooseNewBehavior()
	{
		float num = 0f;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		if (!GhostReactorManager.AggroDisabled && vrrig != null)
		{
			this.investigateLocation = null;
			float num2 = ((this.currBehavior == GREnemySummoner.Behavior.KeepDistance) ? (this.keepDistanceThreshold + 1f) : this.keepDistanceThreshold);
			if (num < num2 * num2)
			{
				this.SetBehavior(GREnemySummoner.Behavior.KeepDistance, false);
				return;
			}
			if (this.CanSummon())
			{
				this.SetBehavior(GREnemySummoner.Behavior.Summon, false);
				return;
			}
			float num3 = this.tooFarDistanceThreshold * this.tooFarDistanceThreshold;
			if (num > num3)
			{
				this.SetBehavior(GREnemySummoner.Behavior.MoveToTarget, false);
				return;
			}
			this.SetBehavior(GREnemySummoner.Behavior.Idle, false);
			return;
		}
		else
		{
			this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
			if (this.investigateLocation != null)
			{
				this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
				this.SetBehavior(GREnemySummoner.Behavior.Investigate, false);
				return;
			}
			double num4 = Time.timeAsDouble - this.abilityIdle.startTime;
			if (this.currBehavior == GREnemySummoner.Behavior.Idle && num4 < (double)this.idleDuration)
			{
				this.SetBehavior(GREnemySummoner.Behavior.Idle, false);
				return;
			}
			this.SetBehavior(GREnemySummoner.Behavior.Wander, false);
			return;
		}
	}

	// Token: 0x06003253 RID: 12883 RVA: 0x00113DB4 File Offset: 0x00111FB4
	public void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06003254 RID: 12884 RVA: 0x00113DD4 File Offset: 0x00111FD4
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			this.abilityIdle.UpdateAuthority(dt);
			return;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.UpdateAuthority(dt);
			return;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.UpdateAuthority(dt);
			if (this.abilityStagger.IsDone())
			{
				this.SetBehavior(GREnemySummoner.Behavior.Wander, false);
				return;
			}
			break;
		case GREnemySummoner.Behavior.Destroyed:
			this.abilityDie.UpdateAuthority(dt);
			return;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.UpdateAuthority(dt);
			return;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.UpdateAuthority(dt);
			return;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.UpdateAuthority(dt);
			return;
		case GREnemySummoner.Behavior.Investigate:
			this.abilityInvestigate.UpdateAuthority(dt);
			return;
		case GREnemySummoner.Behavior.Jump:
			this.abilityJump.UpdateAuthority(dt);
			if (this.abilityJump.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemySummoner.Behavior.Flashed:
			this.abilityFlashed.UpdateAuthority(dt);
			if (this.abilityFlashed.IsDone())
			{
				this.ChooseNewBehavior();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06003255 RID: 12885 RVA: 0x00113ED8 File Offset: 0x001120D8
	public void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Destroyed:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Flashed:
			this.abilityFlashed.UpdateRemote(dt);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003256 RID: 12886 RVA: 0x00113F90 File Offset: 0x00112190
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte b = (byte)this.currBehavior;
		byte b2 = (byte)this.currBodyState;
		writer.Write(b);
		writer.Write(this.hp);
		writer.Write(b2);
	}

	// Token: 0x06003257 RID: 12887 RVA: 0x00113FC8 File Offset: 0x001121C8
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemySummoner.Behavior behavior = (GREnemySummoner.Behavior)reader.ReadByte();
		int num = reader.ReadInt32();
		GREnemySummoner.BodyState bodyState = (GREnemySummoner.BodyState)reader.ReadByte();
		this.SetHP(num);
		this.SetBehavior(behavior, true);
		this.SetBodyState(bodyState, true);
	}

	// Token: 0x06003258 RID: 12888 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06003259 RID: 12889 RVA: 0x00114004 File Offset: 0x00112204
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

	// Token: 0x0600325A RID: 12890 RVA: 0x00114068 File Offset: 0x00112268
	private void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBehavior == GREnemySummoner.Behavior.Destroyed)
		{
			return;
		}
		if (this.currBodyState != GREnemySummoner.BodyState.Bones)
		{
			if (this.currBodyState == GREnemySummoner.BodyState.Shell && this.armor != null)
			{
				this.armor.PlayBlockFx(hit.hitEntityPosition);
			}
			return;
		}
		this.hp -= hit.hitAmount;
		if (this.hp <= 0)
		{
			this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hit.hitByEntityId));
			this.abilityDie.SetStaggerVelocity(hit.hitImpulse);
			this.SetBehavior(GREnemySummoner.Behavior.Destroyed, false);
			return;
		}
		this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
		this.TrySetBehavior(GREnemySummoner.Behavior.Stagger);
	}

	// Token: 0x0600325B RID: 12891 RVA: 0x0011411C File Offset: 0x0011231C
	public void InstantDeath()
	{
		this.hp = 0;
		this.SetBehavior(GREnemySummoner.Behavior.Destroyed, false);
	}

	// Token: 0x0600325C RID: 12892 RVA: 0x00114130 File Offset: 0x00112330
	private void OnHitByFlash(GRTool tool, GameHitData hit)
	{
		this.abilityFlashed.SetStaggerVelocity(hit.hitImpulse);
		if (this.currBodyState == GREnemySummoner.BodyState.Shell)
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
				this.SetBodyState(GREnemySummoner.BodyState.Bones, false);
				if (tool.gameEntity.IsHeldByLocalPlayer())
				{
					PlayerGameEvents.MiscEvent("GRArmorBreak_" + base.name, 1);
				}
				if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage3))
				{
					this.armor.FragmentArmor();
				}
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
		this.TrySetBehavior(GREnemySummoner.Behavior.Flashed);
	}

	// Token: 0x0600325D RID: 12893 RVA: 0x00114258 File Offset: 0x00112458
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	// Token: 0x0600325E RID: 12894 RVA: 0x00114264 File Offset: 0x00112464
	private void OnTriggerEnter(Collider collider)
	{
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			GRPlayer component = attachedRigidbody.GetComponent<GRPlayer>();
			if (component != null && component.gamePlayer.IsLocal())
			{
				GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Phantom, this.entity.id, component, base.transform.position);
			}
			GRBreakable component2 = attachedRigidbody.GetComponent<GRBreakable>();
			GameHittable component3 = attachedRigidbody.GetComponent<GameHittable>();
			if (component2 != null && component3 != null)
			{
				GameHitData gameHitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = component3.gameEntity.id,
					hitByEntityId = this.entity.id,
					hitEntityPosition = component2.transform.position,
					hitImpulse = Vector3.zero,
					hitPosition = component2.transform.position,
					hittablePoint = component3.FindHittablePoint(collider)
				};
				component3.RequestHit(gameHitData);
			}
		}
	}

	// Token: 0x0600325F RID: 12895 RVA: 0x0011436C File Offset: 0x0011256C
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemySummoner.BodyState.Destroyed:
			this.armor.SetHp(0);
			return;
		case GREnemySummoner.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			GREnemy.HideObjects(this.bonesStateVisibleObjects, false);
			GREnemy.HideObjects(this.alwaysVisibleObjects, false);
			return;
		case GREnemySummoner.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, false);
			GREnemy.HideObjects(this.bonesStateVisibleObjects, true);
			GREnemy.HideObjects(this.alwaysVisibleObjects, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003260 RID: 12896 RVA: 0x00114420 File Offset: 0x00112620
	public void SetBodyState(GREnemySummoner.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemySummoner.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemySummoner.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemySummoner.BodyState.Destroyed:
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemySummoner.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemySummoner.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x06003261 RID: 12897 RVA: 0x001144F8 File Offset: 0x001126F8
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		strings.Add(string.Format("Nearby rigs: <color=\"yellow\">{0}<color=\"white\">", this.senseNearby.rigsNearby.Count));
		strings.Add(string.Format("Spawned entities: <color=\"yellow\">{0}<color=\"white\">", this.trackedEntities.Count));
	}

	// Token: 0x06003262 RID: 12898 RVA: 0x00114580 File Offset: 0x00112780
	public void AddTrackedEntity(GameEntity entityToTrack)
	{
		int netId = entityToTrack.GetNetId();
		this.trackedEntities.AddIfNew(netId);
	}

	// Token: 0x06003263 RID: 12899 RVA: 0x001145A0 File Offset: 0x001127A0
	public void RemoveTrackedEntity(GameEntity entityToRemove)
	{
		int netId = entityToRemove.GetNetId();
		if (this.trackedEntities.Contains(netId))
		{
			this.trackedEntities.Remove(netId);
		}
	}

	// Token: 0x06003264 RID: 12900 RVA: 0x001145CF File Offset: 0x001127CF
	public void OnSummonedEntityInit(GameEntity entity)
	{
		this.AddTrackedEntity(entity);
	}

	// Token: 0x06003265 RID: 12901 RVA: 0x001145D8 File Offset: 0x001127D8
	public void OnSummonedEntityDestroy(GameEntity entity)
	{
		this.RemoveTrackedEntity(entity);
	}

	// Token: 0x0400410D RID: 16653
	private GameEntity entity;

	// Token: 0x0400410E RID: 16654
	private GameAgent agent;

	// Token: 0x0400410F RID: 16655
	private GREnemy enemy;

	// Token: 0x04004110 RID: 16656
	public GRArmorEnemy armor;

	// Token: 0x04004111 RID: 16657
	public GRAttributes attributes;

	// Token: 0x04004112 RID: 16658
	public Animation anim;

	// Token: 0x04004113 RID: 16659
	public GRSenseNearby senseNearby;

	// Token: 0x04004114 RID: 16660
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04004115 RID: 16661
	public GRAbilityIdle abilityIdle;

	// Token: 0x04004116 RID: 16662
	public GRAbilityWander abilityWander;

	// Token: 0x04004117 RID: 16663
	public GRAbilityAttackJump abilityAttack;

	// Token: 0x04004118 RID: 16664
	public GRAbilityStagger abilityStagger;

	// Token: 0x04004119 RID: 16665
	public GRAbilityDie abilityDie;

	// Token: 0x0400411A RID: 16666
	public GRAbilitySummon abilitySummon;

	// Token: 0x0400411B RID: 16667
	public GRAbilityKeepDistance abilityKeepDistance;

	// Token: 0x0400411C RID: 16668
	public GRAbilityMoveToTarget abilityMoveToTarget;

	// Token: 0x0400411D RID: 16669
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x0400411E RID: 16670
	public GRAbilityJump abilityJump;

	// Token: 0x0400411F RID: 16671
	public GRAbilityStagger abilityFlashed;

	// Token: 0x04004120 RID: 16672
	public AbilitySound soundWander;

	// Token: 0x04004121 RID: 16673
	public AbilitySound soundAttack;

	// Token: 0x04004122 RID: 16674
	public GameLight summonLight;

	// Token: 0x04004123 RID: 16675
	public List<Renderer> bones;

	// Token: 0x04004124 RID: 16676
	public List<Renderer> always;

	// Token: 0x04004125 RID: 16677
	public List<GameObject> bonesStateVisibleObjects;

	// Token: 0x04004126 RID: 16678
	public List<GameObject> alwaysVisibleObjects;

	// Token: 0x04004127 RID: 16679
	public Transform coreMarker;

	// Token: 0x04004128 RID: 16680
	public GRCollectible corePrefab;

	// Token: 0x04004129 RID: 16681
	public Transform headTransform;

	// Token: 0x0400412A RID: 16682
	public float attackRange = 2f;

	// Token: 0x0400412B RID: 16683
	public List<VRRig> rigsNearby;

	// Token: 0x0400412C RID: 16684
	public NavMeshAgent navAgent;

	// Token: 0x0400412D RID: 16685
	public AudioSource audioSource;

	// Token: 0x0400412E RID: 16686
	public float idleDuration = 2f;

	// Token: 0x0400412F RID: 16687
	public float keepDistanceThreshold = 3f;

	// Token: 0x04004130 RID: 16688
	public float tooFarDistanceThreshold = 5f;

	// Token: 0x04004131 RID: 16689
	public double lastSummonTime;

	// Token: 0x04004132 RID: 16690
	public float minSummonInterval = 4f;

	// Token: 0x04004133 RID: 16691
	public int maxSimultaneousSummonedEntities = 3;

	// Token: 0x04004134 RID: 16692
	public float hearingRadius = 7f;

	// Token: 0x04004135 RID: 16693
	[ReadOnly]
	public int hp;

	// Token: 0x04004136 RID: 16694
	[ReadOnly]
	public GREnemySummoner.Behavior currBehavior;

	// Token: 0x04004137 RID: 16695
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x04004138 RID: 16696
	[ReadOnly]
	public GREnemySummoner.BodyState currBodyState;

	// Token: 0x04004139 RID: 16697
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x0400413A RID: 16698
	[ReadOnly]
	public double behaviorStartTime;

	// Token: 0x0400413B RID: 16699
	private Rigidbody rigidBody;

	// Token: 0x0400413C RID: 16700
	private List<Collider> colliders;

	// Token: 0x0400413D RID: 16701
	private List<int> trackedEntities;

	// Token: 0x0400413E RID: 16702
	private Vector3? investigateLocation;

	// Token: 0x0400413F RID: 16703
	private float lastUpdateTime;

	// Token: 0x04004140 RID: 16704
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x020007AD RID: 1965
	public enum Behavior
	{
		// Token: 0x04004142 RID: 16706
		Idle,
		// Token: 0x04004143 RID: 16707
		Wander,
		// Token: 0x04004144 RID: 16708
		Stagger,
		// Token: 0x04004145 RID: 16709
		Destroyed,
		// Token: 0x04004146 RID: 16710
		Summon,
		// Token: 0x04004147 RID: 16711
		KeepDistance,
		// Token: 0x04004148 RID: 16712
		MoveToTarget,
		// Token: 0x04004149 RID: 16713
		Investigate,
		// Token: 0x0400414A RID: 16714
		Jump,
		// Token: 0x0400414B RID: 16715
		Flashed,
		// Token: 0x0400414C RID: 16716
		Count
	}

	// Token: 0x020007AE RID: 1966
	public enum BodyState
	{
		// Token: 0x0400414E RID: 16718
		Destroyed,
		// Token: 0x0400414F RID: 16719
		Bones,
		// Token: 0x04004150 RID: 16720
		Shell,
		// Token: 0x04004151 RID: 16721
		Count
	}
}
